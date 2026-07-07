using Microsoft.IdentityModel.Tokens;

namespace VehicleServiceBooking.Auth.Services;

/// <summary>
/// Exposes signing key material and JWKS payload generation.
/// </summary>
public interface ISigningKeyProvider
{
    /// <summary>
    /// Gets the active signing key used for token signing.
    /// </summary>
    SecurityKey SigningKey { get; }

    /// <summary>
    /// Gets the key identifier for the active signing key.
    /// </summary>
    string KeyId { get; }

    /// <summary>
    /// Gets currently valid verification keys, optionally filtered by kid.
    /// </summary>
    /// <param name="kid">Optional key identifier filter.</param>
    /// <returns>Collection of usable verification keys.</returns>
    IReadOnlyCollection<SecurityKey> GetValidationKeys(string? kid = null);

    /// <summary>
    /// Returns public JWK information for token validation clients.
    /// </summary>
    object GetPublicJwks();

    /// <summary>
    /// Rotates the active signing key and returns the new kid.
    /// </summary>
    /// <returns>New active key identifier.</returns>
    string RotateKey();
}
