using System.Security.Cryptography;
using System.Text;

namespace VehicleServiceBooking.Auth.Services;

/// <summary>
/// Provides PBKDF2-based password hashing and verification.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    /// <inheritdoc />
    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        var result = new byte[salt.Length + hash.Length];
        Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
        Buffer.BlockCopy(hash, 0, result, salt.Length, hash.Length);
        return Convert.ToBase64String(result);
    }

    /// <inheritdoc />
    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var bytes = Convert.FromBase64String(hashedPassword);
        var salt = bytes[^48..^32];
        var storedHash = bytes[^32..];
        var derivedHash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(providedPassword),
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);
        return CryptographicOperations.FixedTimeEquals(storedHash, derivedHash);
    }
}
