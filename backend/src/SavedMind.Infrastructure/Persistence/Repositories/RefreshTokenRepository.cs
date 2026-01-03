using Microsoft.EntityFrameworkCore;
using SavedMind.Domain.Abstractions;
using SavedMind.Domain.Entities;

namespace SavedMind.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        await context.RefreshTokens.AddAsync(token, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == tokenHash, cancellationToken);
    }

    public async Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        context.RefreshTokens.Update(token);
        await context.SaveChangesAsync(cancellationToken);
    }
}