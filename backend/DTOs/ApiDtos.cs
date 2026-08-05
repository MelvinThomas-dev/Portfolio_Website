using System.ComponentModel.DataAnnotations;
using Portfolio.Api.Models;

namespace Portfolio.Api.DTOs;

public class ContactRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required, MaxLength(5000)]
    public string Message { get; set; } = string.Empty;

    public string? Referrer { get; set; }
    public string? LandingPage { get; set; }
    public string? UtmSource { get; set; }
    public string? UtmMedium { get; set; }
    public string? UtmCampaign { get; set; }
    public string? DeviceType { get; set; }
    public string? SessionId { get; set; }
}

public class VisitRequest
{
    [Required, MaxLength(500)]
    public string PagePath { get; set; } = string.Empty;

    public string? Referrer { get; set; }
    public string? UtmSource { get; set; }
    public string? UtmMedium { get; set; }
    public string? UtmCampaign { get; set; }
    public string? DeviceType { get; set; }
    public string? SessionId { get; set; }
}

public class LoginRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class DashboardStats
{
    public VisitSummary Visits { get; set; } = new();
    public List<LabelCount> TopCountries { get; set; } = [];
    public List<LabelCount> TopCities { get; set; } = [];
    public List<LabelCount> Countries { get; set; } = [];
    public List<CityCount> Cities { get; set; } = [];
    public List<LabelCount> Devices { get; set; } = [];
    public List<LabelCount> TrafficSources { get; set; } = [];
    public List<ContactSubmissionDto> Submissions { get; set; } = [];
}

public class CityCount
{
    public string City { get; set; } = string.Empty;
    public string? Country { get; set; }
    public int Count { get; set; }
}

public class VisitSummary
{
    public int TotalAllTime { get; set; }
    public int UniqueSessionsAllTime { get; set; }
    public int Total7Days { get; set; }
    public int UniqueSessions7Days { get; set; }
    public int Total30Days { get; set; }
    public int UniqueSessions30Days { get; set; }
}

public class LabelCount
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ContactSubmissionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? DeviceType { get; set; }
    public string? TrafficSource { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GeoLocationResult
{
    public string? Country { get; set; }
    public string? Region { get; set; }
    public string? City { get; set; }
}
