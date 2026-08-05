using Portfolio.Api.Models;

namespace Portfolio.Api.Services;

public interface IEmailService
{
    Task SendContactNotificationAsync(ContactSubmission submission, CancellationToken cancellationToken = default);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendContactNotificationAsync(ContactSubmission submission, CancellationToken cancellationToken = default)
    {
        var provider = _configuration["Email:Provider"] ?? "Console";
        var toEmail = _configuration["Email:ToEmail"] ?? "melthomas220@gmail.com";
        var body = BuildEmailBody(submission);

        if (provider.Equals("Console", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation(
                "Contact email (console mode) — configure SENDGRID_API_KEY to send real email.\nTo: {To}\nSubject: Portfolio Contact: {Subject}\n{Body}",
                toEmail,
                submission.Subject,
                body);
            return;
        }

        var apiKey = _configuration["Email:SendGridApiKey"] ?? Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning(
                "SENDGRID_API_KEY is not configured. Contact email was NOT sent. Set Email:SendGridApiKey or SENDGRID_API_KEY environment variable.");
            _logger.LogInformation(
                "Contact email (not sent — missing API key):\nTo: {To}\nSubject: Portfolio Contact: {Subject}\n{Body}",
                toEmail,
                submission.Subject,
                body);
            return;
        }

        var fromEmail = _configuration["Email:FromEmail"] ?? "noreply@portfolio.local";
        var fromName = _configuration["Email:FromName"] ?? "Portfolio Contact";

        var client = new SendGrid.SendGridClient(apiKey);
        var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(
            new SendGrid.Helpers.Mail.EmailAddress(fromEmail, fromName),
            new SendGrid.Helpers.Mail.EmailAddress(toEmail),
            $"Portfolio Contact: {submission.Subject}",
            body,
            $"<pre>{System.Net.WebUtility.HtmlEncode(body)}</pre>");

        var response = await client.SendEmailAsync(msg, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Body.ReadAsStringAsync(cancellationToken);
            _logger.LogError("SendGrid failed: {Status} {Body}", response.StatusCode, responseBody);
            throw new InvalidOperationException("Failed to send contact email.");
        }
    }

    private static string BuildEmailBody(ContactSubmission s)
    {
        return $"""
            From: {s.Name} ({s.Email})
            Subject: {s.Subject}

            Message:
            {s.Message}

            --- Visitor Metadata ---
            Location: {FormatLocation(s.City, s.Region, s.Country)}
            Device: {s.DeviceType ?? "Unknown"}
            Traffic: {s.TrafficSource ?? "Unknown"} (utm_source={s.UtmSource ?? "n/a"}, utm_medium={s.UtmMedium ?? "n/a"})
            Referrer: {s.Referrer ?? "none"}
            Landing Page: {s.LandingPage ?? "n/a"}
            Session: {s.SessionId ?? "n/a"}
            IP: {s.IpAddress ?? "n/a"}
            User-Agent: {s.UserAgent ?? "n/a"}
            Submitted: {s.CreatedAt:u}
            """;
    }

    private static string FormatLocation(string? city, string? region, string? country)
    {
        var parts = new[] { city, region, country }.Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
        return parts.Count == 0 ? "Unknown" : string.Join(", ", parts);
    }
}
