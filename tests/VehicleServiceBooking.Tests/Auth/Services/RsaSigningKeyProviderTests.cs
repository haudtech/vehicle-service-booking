using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using VehicleServiceBooking.Auth.Configuration;
using VehicleServiceBooking.Auth.Services;
using Xunit;

namespace VehicleServiceBooking.Tests.Auth.Services;

public class RsaSigningKeyProviderTests
{
    [Fact]
    public void RotateKey_WhenOverlapNotExpired_KeepsPreviousKeyForValidationAndJwks()
    {
        var clock = new AdjustableTimeProvider(new DateTimeOffset(2026, 7, 6, 12, 0, 0, TimeSpan.Zero));
        var options = new KeyRotationOptions
        {
            EnableRotation = true,
            OverlapMinutes = 60,
            MaxPublishedKeys = 3
        };

        var sut = new RsaSigningKeyProvider(options, NullLogger<RsaSigningKeyProvider>.Instance, clock);
        var oldKid = sut.KeyId;

        var newKid = sut.RotateKey();
        newKid.Should().NotBe(oldKid);

        var keys = sut.GetValidationKeys();
        keys.Select(k => k.KeyId).Should().Contain(new[] { oldKid, newKid });

        var jwksKids = ReadKidsFromJwks(sut.GetPublicJwks());
        jwksKids.Should().Contain(new[] { oldKid, newKid });
    }

    [Fact]
    public void RotateKey_WhenOverlapExpired_RemovesPreviousKeyFromValidationAndJwks()
    {
        var clock = new AdjustableTimeProvider(new DateTimeOffset(2026, 7, 6, 12, 0, 0, TimeSpan.Zero));
        var options = new KeyRotationOptions
        {
            EnableRotation = true,
            OverlapMinutes = 1,
            MaxPublishedKeys = 3
        };

        var sut = new RsaSigningKeyProvider(options, NullLogger<RsaSigningKeyProvider>.Instance, clock);
        var oldKid = sut.KeyId;

        var newKid = sut.RotateKey();
        clock.Advance(TimeSpan.FromMinutes(2));

        var keysAfterExpiry = sut.GetValidationKeys();
        keysAfterExpiry.Select(k => k.KeyId).Should().Contain(newKid);
        keysAfterExpiry.Select(k => k.KeyId).Should().NotContain(oldKid);

        var oldKidLookup = sut.GetValidationKeys(oldKid);
        oldKidLookup.Should().BeEmpty();

        var jwksKids = ReadKidsFromJwks(sut.GetPublicJwks());
        jwksKids.Should().Contain(newKid);
        jwksKids.Should().NotContain(oldKid);
    }

    [Fact]
    public void GetValidationKeys_WithUnknownKid_ReturnsEmpty()
    {
        var options = new KeyRotationOptions
        {
            EnableRotation = true,
            OverlapMinutes = 60,
            MaxPublishedKeys = 2
        };

        var sut = new RsaSigningKeyProvider(options, NullLogger<RsaSigningKeyProvider>.Instance);

        var keys = sut.GetValidationKeys("kid-that-does-not-exist");

        keys.Should().BeEmpty();
    }

    private static IReadOnlyCollection<string> ReadKidsFromJwks(object jwks)
    {
        using var doc = JsonDocument.Parse(JsonSerializer.Serialize(jwks));
        return doc.RootElement
            .GetProperty("keys")
            .EnumerateArray()
            .Select(x => x.GetProperty("kid").GetString())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Cast<string>()
            .ToArray();
    }

    private sealed class AdjustableTimeProvider : TimeProvider
    {
        private DateTimeOffset _current;

        public AdjustableTimeProvider(DateTimeOffset initial)
        {
            _current = initial;
        }

        public override DateTimeOffset GetUtcNow() => _current;

        public void Advance(TimeSpan delta)
        {
            _current = _current.Add(delta);
        }
    }
}
