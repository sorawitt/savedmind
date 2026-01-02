using System.Security.Cryptography;
using System.Text;

namespace SavedMind.Application.Common.Security;

/// <summary>
/// Utility class for generating and hashing secure tokens.
/// Used for email verification, password reset, etc.
/// </summary>
public static class TokenGenerator
{
    private const int DefaultTokenSizeBytes = 32; // 256 bits

    /// <summary>
    /// Generates a cryptographically secure random token encoded as Base64URL.
    /// </summary>
    public static string GenerateSecureToken(int sizeInBytes = DefaultTokenSizeBytes)
    {
        var bytes = RandomNumberGenerator.GetBytes(sizeInBytes);
        return Base64UrlEncode(bytes);
    }

    /// <summary>
    /// Computes SHA256 hash of the input string and returns it as uppercase hex.
    /// </summary>
    public static string ComputeSha256Hash(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    /// <summary>
    /// Encodes bytes to Base64URL format (RFC 4648).
    /// Safe for use in URLs and filenames.
    /// </summary>
    private static string Base64UrlEncode(byte[] data)
    {
        return Convert.ToBase64String(data)
            .Replace("+", "-")  // Standard Base64 -> Base64URL
            .Replace("/", "_")  // Standard Base64 -> Base64URL
            .TrimEnd('=');      // Padding not required
    }
}
