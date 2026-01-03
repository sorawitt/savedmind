using SavedMind.Domain.Entities;

namespace SavedMind.Domain.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);

    /// <summary>
    /// Get user by ID.
    /// Used by: JwtTokenService.RefreshAccessTokenAsync
    /// </summary>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Update user entity.
    /// Used by: LoginCommandHandler (update failed attempts)
    /// </summary>
    Task UpdateAsync(User user, CancellationToken cancellationToken);
}
