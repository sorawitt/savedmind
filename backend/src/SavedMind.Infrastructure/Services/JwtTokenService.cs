using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SavedMind.Application.Common.Security;
using SavedMind.Domain.Abstractions;
using SavedMind.Domain.Entities;

namespace SavedMind.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUserRepository _users;

    public JwtTokenService(
        IOptions<JwtSettings> settings,
        IRefreshTokenRepository refreshTokens,
        IUserRepository users)
    {
        _settings = settings.Value;
        _refreshTokens = refreshTokens;
        _users = users;
    }

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateRefreshTokenAsync(User user, CancellationToken ct)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var rawToken = Convert.ToBase64String(randomBytes);
        var tokenHash = TokenGenerator.ComputeSha256Hash(rawToken);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpiryDays),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokens.AddAsync(refreshToken, ct);
        return rawToken;
    }

    public async Task<string?> RefreshAccessTokenAsync(string refreshToken, CancellationToken ct)
    {
        var tokenHash = TokenGenerator.ComputeSha256Hash(refreshToken);
        var storedToken = await _refreshTokens.GetByTokenAsync(tokenHash, ct);

        if (storedToken is null) return null;
        if (storedToken.RevokedAt is not null) return null;
        if (storedToken.ExpiresAt < DateTime.UtcNow) return null;

        var user = await _users.GetByIdAsync(storedToken.UserId, ct);
        if (user is null) return null;

        return GenerateAccessToken(user);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct)
    {
        var tokenHash = TokenGenerator.ComputeSha256Hash(refreshToken);
        var storedToken = await _refreshTokens.GetByTokenAsync(tokenHash, ct);

        if (storedToken is not null)
        {
            storedToken.RevokedAt = DateTime.UtcNow;
            await _refreshTokens.UpdateAsync(storedToken, ct);
        }
    }
}
