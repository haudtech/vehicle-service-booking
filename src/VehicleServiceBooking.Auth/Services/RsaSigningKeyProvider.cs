using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace VehicleServiceBooking.Auth.Services;

/// <summary>
/// Provides an in-memory RSA signing key and corresponding public JWKS output.
/// </summary>
public sealed class RsaSigningKeyProvider : ISigningKeyProvider
{
    /// <inheritdoc />
    public SecurityKey SigningKey { get; }

    /// <inheritdoc />
    public string KeyId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RsaSigningKeyProvider"/> class.
    /// </summary>
    public RsaSigningKeyProvider()
    {
        var rsa = RSA.Create(2048);
        KeyId = Guid.NewGuid().ToString("N");
        SigningKey = new RsaSecurityKey(rsa) { KeyId = KeyId };
    }

    /// <inheritdoc />
    public object GetPublicJwks()
    {
        var rsaKey = (RsaSecurityKey)SigningKey;
        var parameters = rsaKey.Rsa!.ExportParameters(false);
        var jwk = new JsonWebKey
        {
            Kid = KeyId,
            Kty = JsonWebAlgorithmsKeyTypes.RSA,
            Use = JsonWebKeyUseNames.Sig,
            Alg = SecurityAlgorithms.RsaSha256,
            E = Base64UrlEncoder.Encode(parameters.Exponent),
            N = Base64UrlEncoder.Encode(parameters.Modulus)
        };

        return new { keys = new[] { jwk } };
    }
}
