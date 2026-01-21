using System.Security.Cryptography;
using System.Text;

namespace Application.Helpers;

/// <summary>
/// Provides utility methods for string operations.
/// </summary>
public class StringHelper
{
    /// <summary>
    /// Generates a cryptographically secure random string.
    /// </summary>
    /// <param name="length">The desired length of the generated string. Must be greater than 0.</param>
    /// <param name="onlyAlphaNumeric">
    /// If <c>true</c>, the generated string contains only letters and digits.
    /// If <c>false</c>, the string is Base64-encoded, which may include symbols.
    /// </param>
    /// <returns>
    /// A random string of the specified length.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="length"/> is less than or equal to 0.</exception>
    /// <remarks>
    /// <para>When <paramref name="onlyAlphaNumeric"/> is <c>false</c>, the method generates a Base64 string, then trims characters like '+', '/', '=' to match the requested length.</para>
    /// <para>When <paramref name="onlyAlphaNumeric"/> is <c>true</c>, only characters from A-Z, a-z, and 0-9 are used.</para>
    /// <para>The method uses <see cref="RandomNumberGenerator"/> to ensure cryptographic randomness.</para>
    /// </remarks>
    public static string GenerateRandomString(int length, bool onlyAlphaNumeric = false)
    {
        if (length <= 0)
            throw new ArgumentException("Length must be greater than 0.");

        if (!onlyAlphaNumeric)
        {
            // Trả về Base64 (mặc định)
            var randomBytes = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert
                .ToBase64String(randomBytes)
                .Substring(0, length)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "");
        }
        else
        {
            // Chỉ chứa chữ và số
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new StringBuilder();
            using var rng = RandomNumberGenerator.Create();
            var buffer = new byte[sizeof(uint)];

            while (result.Length < length)
            {
                rng.GetBytes(buffer);
                uint num = BitConverter.ToUInt32(buffer, 0);
                result.Append(chars[(int)(num % (uint)chars.Length)]);
            }

            return result.ToString();
        }
    }
}
