namespace SavedMind.Domain.Abstractions;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string toEmail, string verificationLink, CancellationToken ct);
}
