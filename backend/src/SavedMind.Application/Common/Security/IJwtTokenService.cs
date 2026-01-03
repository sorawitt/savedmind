using SavedMind.Domain.Entities;

namespace SavedMind.Application.Common.Security;

/// <summary>
/// Service interface for managing JWT tokens.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generate a JWT access token for the authenticated user.
    /// </summary>
    /// <param name="user">The authenticated user entity</param>
    /// <returns>JWT token string</returns>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generate a refresh token and store its hash in the database.
    /// </summary>
    /// <param name="user">The authenticated user entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Raw refresh token string</returns>
    Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken);

    /// <summary>
    /// Validate refresh token and issue a new access token.
    /// </summary>
    /// <param name="refreshToken">Raw refresh token from client</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New access token, or null if invalid</returns>
    Task<string?> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken);

    /// <summary>
    /// Revoke a refresh token (logout).
    /// </summary>
    /// <param name="refreshToken">Raw refresh token to revoke</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}