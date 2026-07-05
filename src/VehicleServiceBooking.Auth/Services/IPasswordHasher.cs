namespace VehicleServiceBooking.Auth.Services;

/// <summary>
/// Defines password hashing and verification operations.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plaintext password.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a plaintext password against a stored hash.
    /// </summary>
    bool VerifyPassword(string hashedPassword, string providedPassword);
}
