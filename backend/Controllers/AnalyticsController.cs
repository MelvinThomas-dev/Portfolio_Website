using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;
using Portfolio.Api.DTOs;
using Portfolio.Api.Models;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private static readonly TimeSpan VisitDedupWindow = TimeSpan.FromMinutes(30);

    private readonly PortfolioDbContext _db;
    private readonly IVisitorMetadataService _metadataService;

    public AnalyticsController(
        PortfolioDbContext db,
        IVisitorMetadataService metadataService)
    {
        _db = db;
        _metadataService = metadataService;
    }

    [HttpPost("visit")]
    public async Task<IActionResult> RecordVisit([FromBody] VisitRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var sessionKey = request.SessionId ?? "anonymous";
        var dedupSince = DateTime.UtcNow.Subtract(VisitDedupWindow);

        var alreadyRecorded = await _db.PageVisits.AsNoTracking().AnyAsync(
            v => v.SessionId == sessionKey
                 && v.PagePath == request.PagePath
                 && v.CreatedAt >= dedupSince,
            cancellationToken);

        if (alreadyRecorded)
            return Ok(new { message = "Visit recorded (deduplicated)." });

        var visit = new PageVisit
        {
            PagePath = request.PagePath,
            Referrer = request.Referrer,
            UtmSource = request.UtmSource,
            UtmMedium = request.UtmMedium,
            UtmCampaign = request.UtmCampaign,
            DeviceType = request.DeviceType,
            SessionId = request.SessionId,
            CreatedAt = DateTime.UtcNow
        };

        await _metadataService.EnrichVisitAsync(visit, HttpContext, cancellationToken);

        _db.PageVisits.Add(visit);
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new { message = "Visit recorded." });
    }

    [Authorize]
    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardStats>> GetDashboard(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var sevenDaysAgo = now.AddDays(-7);
        var thirtyDaysAgo = now.AddDays(-30);

        var visits = await _db.PageVisits.AsNoTracking().ToListAsync(cancellationToken);
        var submissions = await _db.ContactSubmissions
            .AsNoTracking()
            .OrderByDescending(s => s.CreatedAt)
            .Take(100)
            .ToListAsync(cancellationToken);

        var allCountries = visits
            .GroupBy(v => GeoLocationService.NormalizeLocationLabel(v.Country))
            .Select(g => new LabelCount { Label = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Label)
            .ToList();

        var allCities = visits
            .GroupBy(v => new
            {
                City = GeoLocationService.NormalizeLocationLabel(v.City),
                Country = GeoLocationService.NormalizeLocationLabel(v.Country)
            })
            .Select(g => new CityCount
            {
                City = g.Key.City,
                Country = g.Key.Country,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.City)
            .ToList();

        var stats = new DashboardStats
        {
            Visits = new VisitSummary
            {
                TotalAllTime = visits.Count,
                UniqueSessionsAllTime = visits.Where(v => !string.IsNullOrWhiteSpace(v.SessionId)).Select(v => v.SessionId!).Distinct().Count(),
                Total7Days = visits.Count(v => v.CreatedAt >= sevenDaysAgo),
                UniqueSessions7Days = visits.Where(v => v.CreatedAt >= sevenDaysAgo && !string.IsNullOrWhiteSpace(v.SessionId)).Select(v => v.SessionId!).Distinct().Count(),
                Total30Days = visits.Count(v => v.CreatedAt >= thirtyDaysAgo),
                UniqueSessions30Days = visits.Where(v => v.CreatedAt >= thirtyDaysAgo && !string.IsNullOrWhiteSpace(v.SessionId)).Select(v => v.SessionId!).Distinct().Count()
            },
            Countries = allCountries,
            Cities = allCities,
            TopCountries = allCountries.Take(10).ToList(),
            TopCities = allCities
                .Take(10)
                .Select(c => new LabelCount { Label = c.City, Count = c.Count })
                .ToList(),
            Devices = visits
                .GroupBy(v => string.IsNullOrWhiteSpace(v.DeviceType) ? "Unknown" : v.DeviceType!)
                .Select(g => new LabelCount { Label = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList(),
            TrafficSources = visits
                .GroupBy(v => string.IsNullOrWhiteSpace(v.TrafficSource) ? "Unknown" : v.TrafficSource!)
                .Select(g => new LabelCount { Label = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList(),
            Submissions = submissions.Select(s => new ContactSubmissionDto
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                Subject = s.Subject,
                Message = s.Message,
                Location = FormatLocation(s.City, s.Region, s.Country),
                DeviceType = s.DeviceType,
                TrafficSource = s.TrafficSource,
                CreatedAt = s.CreatedAt
            }).ToList()
        };

        return Ok(stats);
    }

    private static string FormatLocation(string? city, string? region, string? country)
    {
        var parts = new[]
        {
            GeoLocationService.NormalizeLocationLabel(city),
            string.IsNullOrWhiteSpace(region) || region.Equals("Local", StringComparison.OrdinalIgnoreCase) ? null : region,
            GeoLocationService.NormalizeLocationLabel(country)
        }
        .Where(p => !string.IsNullOrWhiteSpace(p) && p != "Unknown")
        .Distinct()
        .ToList();

        return parts.Count == 0 ? "Unknown" : string.Join(", ", parts);
    }
}
