using SavedMind.Domain.Abstractions;

namespace SavedMind.Infrastructure.Security;

/// <summary>
/// BCrypt implementation of password hashing.
/// Cost factor 12 = ~250ms per hash (good balance of security vs speed).
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string password)
    {
        // BCrypt automatically generates a random salt and embeds it in the hash
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
    }

    public bool Verify(string password, string passwordHash)
    {
        // BCrypt extracts the salt from the hash and compares
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}