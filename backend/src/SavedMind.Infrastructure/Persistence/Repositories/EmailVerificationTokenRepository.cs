using SavedMind.Domain.Abstractions;
using SavedMind.Domain.Entities;

namespace SavedMind.Infrastructure.Persistence.Repositories;

public class EmailVerificationTokenRepository(AppDbContext context) : IEmailVerificationTokenRepository
{
    public async Task AddAsync(EmailVerificationToken token, CancellationToken ct)
    {
        await context.Set<EmailVerificationToken>().AddAsync(token, ct);
        await context.SaveChangesAsync(ct);
    }
}