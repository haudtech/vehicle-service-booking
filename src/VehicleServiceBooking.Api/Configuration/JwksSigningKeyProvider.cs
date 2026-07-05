using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace VehicleServiceBooking.Api.Configuration;

public interface IJwksSigningKeyProvider
{
    IReadOnlyCollection<SecurityKey> GetSigningKeys(string? kid = null);
}

public sealed class JwksSigningKeyProvider : IJwksSigningKeyProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthJwtOptions _options;
    private readonly ILogger<JwksSigningKeyProvider> _logger;
    private readonly object _syncRoot = new();

    private DateTime _lastRefreshUtc = DateTime.MinValue;
    private List<SecurityKey> _cachedKeys = new();

    public JwksSigningKeyProvider(
        IHttpClientFactory httpClientFactory,
        AuthJwtOptions options,
        ILogger<JwksSigningKeyProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
        _logger = logger;
    }

    public IReadOnlyCollection<SecurityKey> GetSigningKeys(string? kid = null)
    {
        EnsureKeysLoaded();

        if (string.IsNullOrWhiteSpace(kid))
        {
            return _cachedKeys;
        }

        var matching = _cachedKeys
            .Where(key => string.Equals(key.KeyId, kid, StringComparison.Ordinal))
            .ToList();

        return matching.Count > 0 ? matching : _cachedKeys;
    }

    private void EnsureKeysLoaded()
    {
        var cacheDuration = TimeSpan.FromMinutes(Math.Max(1, _options.JwksCacheMinutes));
        if (_cachedKeys.Count > 0 && DateTime.UtcNow - _lastRefreshUtc < cacheDuration)
        {
            return;
        }

        lock (_syncRoot)
        {
            if (_cachedKeys.Count > 0 && DateTime.UtcNow - _lastRefreshUtc < cacheDuration)
            {
                return;
            }

            var client = _httpClientFactory.CreateClient("auth-jwks");
            var jwksJson = client.GetStringAsync(_options.JwksUrl, CancellationToken.None)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            var jwks = new JsonWebKeySet(jwksJson);
            var keys = jwks.GetSigningKeys();

            if (keys.Count == 0)
            {
                throw new InvalidOperationException("No signing keys were returned from JWKS endpoint.");
            }

            _cachedKeys = keys.ToList();
            _lastRefreshUtc = DateTime.UtcNow;

            _logger.LogInformation("Refreshed {Count} JWKS signing keys from {JwksUrl}", _cachedKeys.Count, _options.JwksUrl);
        }
    }
}