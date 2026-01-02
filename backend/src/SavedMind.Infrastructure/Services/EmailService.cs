using Microsoft.Extensions.Logging;
using SavedMind.Domain.Abstractions;

namespace SavedMind.Infrastructure.Services;

/// <summary>
/// Mock email service that logs emails instead of sending them.
/// Replace with real implementation (SendGrid, SMTP, etc.) in production.
/// </summary>
public class EmailService(ILogger<EmailService> logger) : IEmailService
{
    public Task SendVerificationEmailAsync(string toEmail, string verificationLink, CancellationToken ct)
    {
        // In production: use SendGrid, Mailgun, AWS SES, etc.
        // For now: just log it
        logger.LogInformation(
            "MOCK EMAIL\n   To: {Email}\n   Link: {Link}",
            toEmail,
            verificationLink);
        return Task.CompletedTask;
    }
}