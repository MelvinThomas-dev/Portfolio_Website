using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Api.Data;
using Portfolio.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

builder.Services.AddDbContext<PortfolioDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(databaseUrl))
    {
        options.UseNpgsql(ParsePostgresUrl(databaseUrl));
    }
    else
    {
        options.UseSqlite(connectionString ?? "Data Source=portfolio.db");
    }
});

builder.Services.AddHttpClient<IGeoLocationService, GeoLocationService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<ITrafficSourceParser, TrafficSourceParser>();
builder.Services.AddSingleton<IDeviceParser, DeviceParser>();
builder.Services.AddSingleton<IRateLimitService, RateLimitService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVisitorMetadataService, VisitorMetadataService>();

var jwtSecret = builder.Configuration["Jwt:Secret"] ?? Environment.GetEnvironmentVariable("JWT_SECRET") ?? string.Empty;
if (string.IsNullOrWhiteSpace(jwtSecret) && builder.Environment.IsDevelopment())
    jwtSecret = "dev-jwt-secret-change-in-production-min-32-chars";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "PortfolioApi",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "PortfolioAdmin",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

var corsOrigins = builder.Configuration["Cors:Origins"]
    ?? Environment.GetEnvironmentVariable("CORS_ORIGIN")
    ?? "http://localhost:5173";

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(corsOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();

static string ParsePostgresUrl(string databaseUrl)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);
    var username = Uri.UnescapeDataString(userInfo[0]);
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
    var host = uri.Host;
    var port = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');
    return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
}
