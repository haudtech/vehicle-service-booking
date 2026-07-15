using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VehicleServiceBooking.Auth.Services;

namespace VehicleServiceBooking.Tests.Auth.Services;

public class DataProtectionAuthenticatorSecretProtectorTests
{
    [Fact]
    public void Protect_ThenTryUnprotect_ShouldRoundTripAndKeepPrefix()
    {
        var sut = CreateSut();

        var protectedValue = sut.Protect("JBSWY3DPEHPK3PXP");
        var ok = sut.TryUnprotect(protectedValue, out var plaintext);

        ok.Should().BeTrue();
        protectedValue.Should().StartWith("enc:v1:");
        plaintext.Should().Be("JBSWY3DPEHPK3PXP");
    }

    [Fact]
    public void TryUnprotect_ShouldSupportLegacyPlaintextSecret()
    {
        var sut = CreateSut();

        var ok = sut.TryUnprotect("JBSWY3DPEHPK3PXP", out var plaintext);

        ok.Should().BeTrue();
        plaintext.Should().Be("JBSWY3DPEHPK3PXP");
    }

    [Fact]
    public void TryUnprotect_ShouldReturnFalseForCorruptedProtectedPayload()
    {
        var sut = CreateSut();

        var ok = sut.TryUnprotect("enc:v1:not-a-valid-payload", out var plaintext);

        ok.Should().BeFalse();
        plaintext.Should().BeEmpty();
    }

    [Fact]
    public void Protect_ShouldThrowForWhitespaceSecret()
    {
        var sut = CreateSut();

        var act = () => sut.Protect("   ");

        act.Should().Throw<ArgumentException>();
    }

    private static DataProtectionAuthenticatorSecretProtector CreateSut()
    {
        var services = new ServiceCollection();
        services.AddDataProtection();
        using var provider = services.BuildServiceProvider();
        return new DataProtectionAuthenticatorSecretProtector(
            provider.GetRequiredService<Microsoft.AspNetCore.DataProtection.IDataProtectionProvider>());
    }
}
