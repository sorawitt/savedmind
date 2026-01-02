using SavedMind.Domain.Entities;

namespace SavedMind.Domain.Abstractions;

public interface IEmailVerificationTokenRepository
{
    Task AddAsync(EmailVerificationToken token, CancellationToken ct);
}
