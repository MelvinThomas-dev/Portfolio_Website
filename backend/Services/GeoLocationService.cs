using System.Text.Json;
using Portfolio.Api.DTOs;

namespace Portfolio.Api.Services;

public interface IGeoLocationService
{
    Task<GeoLocationResult> GetLocationAsync(string? ipAddress, CancellationToken cancellationToken = default);
}

public class GeoLocationService : IGeoLocationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeoLocationService> _logger;

    public GeoLocationService(HttpClient httpClient, IConfiguration configuration, ILogger<GeoLocationService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public const string LocalDevelopmentLabel = "Local Development";

    public async Task<GeoLocationResult> GetLocationAsync(string? ipAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ipAddress) || IsPrivateIp(ipAddress))
            return new GeoLocationResult { Country = LocalDevelopmentLabel, City = LocalDevelopmentLabel };

        var provider = _configuration["GeoLocation:Provider"] ?? "ip-api";

        try
        {
            return provider.ToLowerInvariant() switch
            {
                "ipinfo" => await GetFromIpInfoAsync(ipAddress, cancellationToken),
                _ => await GetFromIpApiAsync(ipAddress, cancellationToken)
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Geo lookup failed for IP {Ip}", ipAddress);
            return new GeoLocationResult();
        }
    }

    private async Task<GeoLocationResult> GetFromIpApiAsync(string ipAddress, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"http://ip-api.com/json/{ipAddress}?fields=status,country,regionName,city", cancellationToken);
        response.EnsureSuccessStatusCode();

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        var root = doc.RootElement;

        if (root.GetProperty("status").GetString() != "success")
            return new GeoLocationResult();

        return new GeoLocationResult
        {
            Country = root.TryGetProperty("country", out var country) ? country.GetString() : null,
            Region = root.TryGetProperty("regionName", out var region) ? region.GetString() : null,
            City = root.TryGetProperty("city", out var city) ? city.GetString() : null
        };
    }

    private async Task<GeoLocationResult> GetFromIpInfoAsync(string ipAddress, CancellationToken cancellationToken)
    {
        var token = _configuration["GeoLocation:IpInfoToken"];
        var url = string.IsNullOrWhiteSpace(token)
            ? $"https://ipinfo.io/{ipAddress}/json"
            : $"https://ipinfo.io/{ipAddress}/json?token={token}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        var root = doc.RootElement;

        return new GeoLocationResult
        {
            Country = root.TryGetProperty("country", out var country) ? country.GetString() : null,
            Region = root.TryGetProperty("region", out var region) ? region.GetString() : null,
            City = root.TryGetProperty("city", out var city) ? city.GetString() : null
        };
    }

    private static bool IsPrivateIp(string ip)
    {
        var normalized = ip.Trim().ToLowerInvariant();

        if (normalized is "127.0.0.1" or "::1" or "unknown" or "localhost")
            return true;

        if (normalized.StartsWith("::ffff:127.") || normalized.StartsWith("::ffff:10.") ||
            normalized.StartsWith("::ffff:192.168."))
            return true;

        if (normalized.StartsWith("fe80:") || normalized.StartsWith("fc") || normalized.StartsWith("fd"))
            return true;

        return normalized.StartsWith("192.168.") || normalized.StartsWith("10.") ||
               normalized.StartsWith("172.16.") || normalized.StartsWith("172.17.") ||
               normalized.StartsWith("172.18.") || normalized.StartsWith("172.19.") ||
               (normalized.StartsWith("172.2") && normalized.Length >= 7 && int.TryParse(normalized.AsSpan(4, 2), out var octet) && octet is >= 16 and <= 31) ||
               normalized.StartsWith("172.30.") || normalized.StartsWith("172.31.");
    }

    public static string NormalizeLocationLabel(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Unknown";

        if (value.Equals("Local", StringComparison.OrdinalIgnoreCase) ||
            value.Equals(LocalDevelopmentLabel, StringComparison.OrdinalIgnoreCase))
            return LocalDevelopmentLabel;

        return value;
    }
}
