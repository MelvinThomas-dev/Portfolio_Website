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
public class ContactController : ControllerBase
{
    private readonly PortfolioDbContext _db;
    private readonly IEmailService _emailService;
    private readonly IVisitorMetadataService _metadataService;
    private readonly IRateLimitService _rateLimitService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ContactController> _logger;

    public ContactController(
        PortfolioDbContext db,
        IEmailService emailService,
        IVisitorMetadataService metadataService,
        IRateLimitService rateLimitService,
        IConfiguration configuration,
        ILogger<ContactController> logger)
    {
        _db = db;
        _emailService = emailService;
        _metadataService = metadataService;
        _rateLimitService = rateLimitService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] ContactRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (!string.IsNullOrWhiteSpace(request.SessionId))
        {
            var alreadySubmitted = await _db.ContactSubmissions
                .AsNoTracking()
                .AnyAsync(s => s.SessionId == request.SessionId, cancellationToken);

            if (alreadySubmitted)
                return Ok(new { message = "Thank you! Your message has been sent." });
        }

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var maxPerHour = int.TryParse(_configuration["RateLimit:ContactMaxPerHour"], out var limit) ? limit : 5;

        if (!_rateLimitService.IsAllowed($"contact:{ip}", maxPerHour, TimeSpan.FromHours(1)))
            return StatusCode(StatusCodes.Status429TooManyRequests, new { message = "Too many contact submissions. Please try again later." });

        var submission = new ContactSubmission
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Subject = request.Subject.Trim(),
            Message = request.Message.Trim(),
            Referrer = request.Referrer,
            LandingPage = request.LandingPage,
            UtmSource = request.UtmSource,
            UtmMedium = request.UtmMedium,
            UtmCampaign = request.UtmCampaign,
            DeviceType = request.DeviceType,
            SessionId = request.SessionId,
            CreatedAt = DateTime.UtcNow
        };

        await _metadataService.EnrichContactAsync(submission, HttpContext, cancellationToken);

        _db.ContactSubmissions.Add(submission);
        await _db.SaveChangesAsync(cancellationToken);

        try
        {
            await _emailService.SendContactNotificationAsync(submission, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send contact email for submission {Id}", submission.Id);
        }

        return Ok(new { message = "Thank you! Your message has been sent.", id = submission.Id });
    }
}
