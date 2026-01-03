using SavedMind.Domain.Entities;

namespace SavedMind.Domain.Abstractions;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken);

    /// <summary>
    /// Find refresh token by its hash.
    /// Used by: ValidateRefreshToken, RevokeRefreshToken
    /// </summary>
    Task<RefreshToken?> GetByTokenAsync(string tokenHash, CancellationToken cancellationToken);

    /// <summary>
    /// Update refresh token (e.g., set RevokedAt).
    /// </summary>
    Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken);
}
