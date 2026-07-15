using System.Security.Cryptography;
using System.Text;

namespace VehicleServiceBooking.Auth.Common.Verification;

/// <summary>
/// Shared utilities for verification token and code generation.
/// </summary>
public static class VerificationUtils
{
    /// <summary>
    /// Generates a random token suitable for email verification links.
    /// </summary>
    /// <returns>Base64-encoded random token.</returns>
    public static string GenerateVerificationToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
    }

    /// <summary>
    /// Generates a six-digit verification code for login challenges.
    /// </summary>
    /// <returns>Numeric code zero-padded to six digits.</returns>
    public static string GenerateLoginVerificationCode()
    {
        return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
    }

    /// <summary>
    /// Hashes a token/code using SHA-256 and hex encoding.
    /// </summary>
    /// <param name="token">Raw token or code.</param>
    /// <returns>Uppercase hex SHA-256 hash string.</returns>
    public static string HashVerificationToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
