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
    /// Returns public JWK information for token validation clients.
    /// </summary>
    object GetPublicJwks();
}
