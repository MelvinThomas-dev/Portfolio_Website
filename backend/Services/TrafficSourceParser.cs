namespace Portfolio.Api.Services;

public interface ITrafficSourceParser
{
    string Parse(string? referrer, string? utmSource, string? utmMedium);
}

public class TrafficSourceParser : ITrafficSourceParser
{
    public string Parse(string? referrer, string? utmSource, string? utmMedium)
    {
        var source = (utmSource ?? string.Empty).ToLowerInvariant();
        var medium = (utmMedium ?? string.Empty).ToLowerInvariant();
        var refUrl = (referrer ?? string.Empty).ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(source))
        {
            if (source.Contains("linkedin")) return "linkedin";
            if (source.Contains("google")) return "google";
            if (source.Contains("github")) return "github";
            if (source.Contains("twitter") || source.Contains("x.com")) return "social";
            return source;
        }

        if (refUrl.Contains("linkedin.com")) return "linkedin";
        if (refUrl.Contains("google.")) return "google";
        if (refUrl.Contains("github.com")) return "github";
        if (refUrl.Contains("bing.com") || refUrl.Contains("duckduckgo")) return "search";
        if (string.IsNullOrWhiteSpace(referrer)) return "direct";

        return "other";
    }
}
