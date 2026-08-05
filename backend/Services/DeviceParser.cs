namespace Portfolio.Api.Services;

public interface IDeviceParser
{
    string Parse(string? userAgent, string? clientDeviceType);
}

public class DeviceParser : IDeviceParser
{
    public string Parse(string? userAgent, string? clientDeviceType)
    {
        if (!string.IsNullOrWhiteSpace(clientDeviceType))
        {
            var normalized = clientDeviceType.ToLowerInvariant();
            if (normalized is "mobile" or "tablet" or "desktop")
                return char.ToUpper(normalized[0]) + normalized[1..];
        }

        var ua = userAgent ?? string.Empty;
        if (ua.Contains("iPad", StringComparison.OrdinalIgnoreCase) ||
            ua.Contains("Tablet", StringComparison.OrdinalIgnoreCase))
            return "Tablet";

        if (ua.Contains("Mobile", StringComparison.OrdinalIgnoreCase) ||
            ua.Contains("Android", StringComparison.OrdinalIgnoreCase) && !ua.Contains("Tablet", StringComparison.OrdinalIgnoreCase))
            return "Mobile";

        return "Desktop";
    }
}
