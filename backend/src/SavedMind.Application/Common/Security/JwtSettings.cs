namespace SavedMind.Application.Common.Security;

/// <summary>
/// Configuration settings for JWT token generation and validation.
/// Maps to "Jwt" section in appsettings.json via IOptions pattern.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Section name in appsettings.json.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Secret key for signing JWT tokens.
    /// Minimum 32 characters (256 bits for HS256).
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Token issuer (who issued the token).
    /// Typically the URL of your API.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Token audience (who the token is for).
    /// Typically the URL of your frontend.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Access token expiry time in minutes.
    /// Default: 15 minutes.
    /// </summary>
    public int AccessTokenExpiryMinutes { get; set; } = 15;

    /// <summary>
    /// Refresh token expiry time in days.
    /// Default: 7 days.
    /// </summary>
    public int RefreshTokenExpiryDays { get; set; } = 7;
}