namespace Portfolio.Api.Models;

public class PageVisit
{
    public int Id { get; set; }
    public string PagePath { get; set; } = string.Empty;
    public string? Referrer { get; set; }
    public string? UtmSource { get; set; }
    public string? UtmMedium { get; set; }
    public string? UtmCampaign { get; set; }
    public string? DeviceType { get; set; }
    public string? SessionId { get; set; }
    public string? IpAddress { get; set; }
    public string? Country { get; set; }
    public string? Region { get; set; }
    public string? City { get; set; }
    public string? TrafficSource { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
