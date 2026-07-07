using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using VehicleServiceBooking.Auth.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace VehicleServiceBooking.Auth.Services;

/// <summary>
/// Provides an in-memory RSA signing key and corresponding public JWKS output.
/// </summary>
public sealed class RsaSigningKeyProvider : ISigningKeyProvider
{
    private readonly object _syncRoot = new();
    private readonly KeyRotationOptions _rotationOptions;
    private readonly ILogger<RsaSigningKeyProvider> _logger;
    private readonly TimeProvider _timeProvider;
    private readonly List<KeyVersion> _keys = new();

    /// <inheritdoc />
    public SecurityKey SigningKey
    {
        get
        {
            lock (_syncRoot)
            {
                return GetActiveKey().SecurityKey;
            }
        }
    }

    /// <inheritdoc />
    public string KeyId
    {
        get
        {
            lock (_syncRoot)
            {
                return GetActiveKey().SecurityKey.KeyId!;
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RsaSigningKeyProvider"/> class.
    /// </summary>
    public RsaSigningKeyProvider(
        KeyRotationOptions rotationOptions,
        ILogger<RsaSigningKeyProvider> logger,
        TimeProvider? timeProvider = null)
    {
        _rotationOptions = rotationOptions;
        _logger = logger;
        _timeProvider = timeProvider ?? TimeProvider.System;

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var firstKey = CreateKeyVersion(now, isActive: true);
        _keys.Add(firstKey);

        _logger.LogInformation("Initialized JWT signing key provider with active kid {Kid}.", firstKey.SecurityKey.KeyId);
    }

    /// <inheritdoc />
    public IReadOnlyCollection<SecurityKey> GetValidationKeys(string? kid = null)
    {
        lock (_syncRoot)
        {
            PruneExpiredKeys(_timeProvider.GetUtcNow().UtcDateTime);

            var activeKeys = _keys.Where(k => k.RetiredAtUtc is null).ToList();
            if (string.IsNullOrWhiteSpace(kid))
            {
                return activeKeys.Select(k => (SecurityKey)k.SecurityKey).ToList();
            }

            var matching = activeKeys
                .Where(k => string.Equals(k.SecurityKey.KeyId, kid, StringComparison.Ordinal))
                .Select(k => (SecurityKey)k.SecurityKey)
                .ToList();

            return matching;
        }
    }

    /// <inheritdoc />
    public object GetPublicJwks()
    {
        lock (_syncRoot)
        {
            PruneExpiredKeys(_timeProvider.GetUtcNow().UtcDateTime);

            var jwks = _keys
                .Where(k => k.RetiredAtUtc is null)
                .OrderByDescending(k => k.ActivatedAtUtc)
                .Take(Math.Max(1, _rotationOptions.MaxPublishedKeys))
                .Select(ToJsonWebKey)
                .ToList();

            return new { keys = jwks };
        }
    }

    /// <inheritdoc />
    public string RotateKey()
    {
        lock (_syncRoot)
        {
            if (!_rotationOptions.EnableRotation)
            {
                _logger.LogWarning("JWT signing key rotation was requested but is disabled by configuration.");
                return GetActiveKey().SecurityKey.KeyId!;
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var active = GetActiveKey();
            active.IsActive = false;
            active.RetireAtUtc = now.AddMinutes(Math.Max(1, _rotationOptions.OverlapMinutes));

            var next = CreateKeyVersion(now, isActive: true);
            _keys.Add(next);

            PruneExpiredKeys(now);

            _logger.LogInformation(
                "Rotated JWT signing key from kid {PreviousKid} to kid {NewKid}. Overlap minutes: {OverlapMinutes}.",
                active.SecurityKey.KeyId,
                next.SecurityKey.KeyId,
                _rotationOptions.OverlapMinutes);

            return next.SecurityKey.KeyId!;
        }
    }

    private static JsonWebKey ToJsonWebKey(KeyVersion version)
    {
        var parameters = version.SecurityKey.Rsa!.ExportParameters(false);
        return new JsonWebKey
        {
            Kid = version.SecurityKey.KeyId,
            Kty = JsonWebAlgorithmsKeyTypes.RSA,
            Use = JsonWebKeyUseNames.Sig,
            Alg = SecurityAlgorithms.RsaSha256,
            E = Base64UrlEncoder.Encode(parameters.Exponent),
            N = Base64UrlEncoder.Encode(parameters.Modulus)
        };
    }

    private KeyVersion GetActiveKey()
    {
        return _keys.First(k => k.IsActive && k.RetiredAtUtc is null);
    }

    private static KeyVersion CreateKeyVersion(DateTime nowUtc, bool isActive)
    {
        var rsa = RSA.Create(2048);
        var keyId = Guid.NewGuid().ToString("N");
        var key = new RsaSecurityKey(rsa)
        {
            KeyId = keyId
        };

        return new KeyVersion
        {
            SecurityKey = key,
            ActivatedAtUtc = nowUtc,
            IsActive = isActive
        };
    }

    private void PruneExpiredKeys(DateTime nowUtc)
    {
        var toRetire = _keys.Where(k => k.RetireAtUtc is not null && k.RetireAtUtc <= nowUtc).ToList();
        foreach (var key in toRetire)
        {
            key.RetiredAtUtc = nowUtc;
            _logger.LogInformation("Retired JWT verification key kid {Kid}.", key.SecurityKey.KeyId);
        }

        _keys.RemoveAll(k => k.RetiredAtUtc is not null);
    }

    private sealed class KeyVersion
    {
        public required RsaSecurityKey SecurityKey { get; init; }
        public required DateTime ActivatedAtUtc { get; init; }
        public DateTime? RetireAtUtc { get; set; }
        public DateTime? RetiredAtUtc { get; set; }
        public bool IsActive { get; set; }
    }
}
