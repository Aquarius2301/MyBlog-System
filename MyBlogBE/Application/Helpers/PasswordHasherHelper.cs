using System.Security.Cryptography;

namespace Application.Helpers;

/// <summary>
/// Provides methods for securely hashing and verifying passwords using PBKDF2 with SHA-256.
/// </summary>
public static class PasswordHasherHelper
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    /// <summary>
    /// Generates a salted hash for the given password using PBKDF2-SHA256.
    /// </summary>
    /// <param name="password">The plain-text password to hash.</param>
    /// <returns>
    /// A string containing the iteration count, salt, and hash, separated by periods:
    /// <c>{iterations}.{salt}.{hash}</c>.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="password"/> is null or empty.</exception>
    /// <remarks>
    /// Uses a 16-byte random salt and 100,000 iterations for PBKDF2 hashing.
    /// The output string can be stored in the database and later used to verify the password.
    /// </remarks>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password required", nameof(password));

        byte[] salt = new byte[SaltSize];
        RandomNumberGenerator.Fill(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256
        );
        byte[] hash = pbkdf2.GetBytes(HashSize);

        string saltB64 = Convert.ToBase64String(salt);
        string hashB64 = Convert.ToBase64String(hash);
        return $"{Iterations}.{saltB64}.{hashB64}";
    }

    /// <summary>
    /// Verifies a plain-text password against a previously stored hash.
    /// </summary>
    /// <param name="password">The plain-text password to verify.</param>
    /// <param name="storedHash">The stored hash string in the format <c>{iterations}.{salt}.{hash}</c>.</param>
    /// <returns>
    /// <c>true</c> if the password matches the stored hash; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="password"/> is null or empty.</exception>
    /// <remarks>
    /// Uses a constant-time comparison to prevent timing attacks.
    /// Returns <c>false</c> if the stored hash is invalid or cannot be parsed.
    /// </remarks>
    public static bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password required", nameof(password));
        if (string.IsNullOrEmpty(storedHash))
            return false;

        var parts = storedHash.Split('.', 3);
        if (parts.Length != 3)
            return false;

        if (!int.TryParse(parts[0], out int iterations))
            return false;
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] storedHashBytes = Convert.FromBase64String(parts[2]);

        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256
        );
        byte[] computedHash = pbkdf2.GetBytes(storedHashBytes.Length);

        return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
    }
}
