using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Api.DTOs;

namespace Portfolio.Api.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public AuthService(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var configuredUsername = _configuration["Admin:Username"]
            ?? Environment.GetEnvironmentVariable("ADMIN_USERNAME")
            ?? "admin";

        if (!string.Equals(request.Username, configuredUsername, StringComparison.Ordinal))
            return Task.FromResult<LoginResponse?>(null);

        if (!VerifyPassword(request.Password))
            return Task.FromResult<LoginResponse?>(null);

        var secret = _configuration["Jwt:Secret"] ?? Environment.GetEnvironmentVariable("JWT_SECRET");
        if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
        {
            if (!_environment.IsDevelopment())
                throw new InvalidOperationException("JWT secret must be at least 32 characters.");

            secret = "dev-jwt-secret-change-in-production-min-32-chars";
        }

        var expirationHours = int.TryParse(_configuration["Jwt:ExpirationHours"], out var hours) ? hours : 24;
        var expiresAt = DateTime.UtcNow.AddHours(expirationHours);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Name, configuredUsername),
                new Claim(ClaimTypes.Role, "Admin")
            ]),
            Expires = expiresAt,
            Issuer = _configuration["Jwt:Issuer"] ?? "PortfolioApi",
            Audience = _configuration["Jwt:Audience"] ?? "PortfolioAdmin",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Task.FromResult<LoginResponse?>(new LoginResponse
        {
            Token = tokenHandler.WriteToken(token),
            ExpiresAt = expiresAt
        });
    }

    private bool VerifyPassword(string password)
    {
        var plainEnv = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
        if (!string.IsNullOrWhiteSpace(plainEnv))
            return password == plainEnv;

        var hash = _configuration["Admin:PasswordHash"] ?? Environment.GetEnvironmentVariable("ADMIN_PASSWORD_HASH");
        if (!string.IsNullOrWhiteSpace(hash))
            return BCrypt.Net.BCrypt.Verify(password, hash);

        if (_environment.IsDevelopment())
            return password == "admin123";

        return false;
    }
}

public interface IRateLimitService
{
    bool IsAllowed(string key, int maxRequests, TimeSpan window);
}

public class RateLimitService : IRateLimitService
{
    private readonly ConcurrentDictionary<string, List<DateTime>> _requests = new();

    public bool IsAllowed(string key, int maxRequests, TimeSpan window)
    {
        var now = DateTime.UtcNow;
        var timestamps = _requests.GetOrAdd(key, _ => []);

        lock (timestamps)
        {
            timestamps.RemoveAll(t => now - t > window);
            if (timestamps.Count >= maxRequests)
                return false;

            timestamps.Add(now);
            return true;
        }
    }
}

public interface IVisitorMetadataService
{
    Task EnrichContactAsync(Models.ContactSubmission submission, HttpContext httpContext, CancellationToken cancellationToken = default);
    Task EnrichVisitAsync(Models.PageVisit visit, HttpContext httpContext, CancellationToken cancellationToken = default);
}

public class VisitorMetadataService : IVisitorMetadataService
{
    private readonly IGeoLocationService _geoLocationService;
    private readonly IDeviceParser _deviceParser;
    private readonly ITrafficSourceParser _trafficSourceParser;

    public VisitorMetadataService(
        IGeoLocationService geoLocationService,
        IDeviceParser deviceParser,
        ITrafficSourceParser trafficSourceParser)
    {
        _geoLocationService = geoLocationService;
        _deviceParser = deviceParser;
        _trafficSourceParser = trafficSourceParser;
    }

    public async Task EnrichContactAsync(Models.ContactSubmission submission, HttpContext httpContext, CancellationToken cancellationToken = default)
    {
        var ip = GetClientIp(httpContext);
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();

        submission.IpAddress = ip;
        submission.UserAgent = userAgent;
        submission.DeviceType = _deviceParser.Parse(userAgent, submission.DeviceType);
        submission.TrafficSource = _trafficSourceParser.Parse(submission.Referrer, submission.UtmSource, submission.UtmMedium);

        var geo = await _geoLocationService.GetLocationAsync(ip, cancellationToken);
        submission.Country = geo.Country;
        submission.Region = geo.Region;
        submission.City = geo.City;
    }

    public async Task EnrichVisitAsync(Models.PageVisit visit, HttpContext httpContext, CancellationToken cancellationToken = default)
    {
        var ip = GetClientIp(httpContext);
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();

        visit.IpAddress = ip;
        visit.UserAgent = userAgent;
        visit.DeviceType = _deviceParser.Parse(userAgent, visit.DeviceType);
        visit.TrafficSource = _trafficSourceParser.Parse(visit.Referrer, visit.UtmSource, visit.UtmMedium);

        var geo = await _geoLocationService.GetLocationAsync(ip, cancellationToken);
        visit.Country = geo.Country;
        visit.Region = geo.Region;
        visit.City = geo.City;
    }

    private static string? GetClientIp(HttpContext httpContext)
    {
        var forwarded = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwarded))
            return forwarded.Split(',')[0].Trim();

        return httpContext.Connection.RemoteIpAddress?.ToString();
    }
}
