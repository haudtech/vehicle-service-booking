using FluentAssertions;
using Moq;
using VehicleServiceBooking.Auth.Configuration;
using VehicleServiceBooking.Auth.Models;
using VehicleServiceBooking.Auth.Models.Requests;
using VehicleServiceBooking.Auth.Repositories.Interfaces;
using VehicleServiceBooking.Auth.Services;

namespace VehicleServiceBooking.Tests.Auth.Services;

public class AuthServiceMfaEncryptionTests
{
    [Fact]
    public async Task StartAuthenticatorSetupAsync_ShouldPersistProtectedSecret()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "alice@example.com",
            IsActive = true,
            IsEmailVerified = true,
            AuthenticatorAppSecret = string.Empty
        };

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        userRepository
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken _) => u);
        userRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var authenticatorSecretProtector = new Mock<IAuthenticatorSecretProtector>();
        authenticatorSecretProtector
            .Setup(x => x.Protect(It.IsAny<string>()))
            .Returns("enc:v1:ciphertext-secret");

        var sut = CreateSut(
            userRepository,
            authenticatorSecretProtector,
            out _,
            out _);

        var result = await sut.StartAuthenticatorSetupAsync(user.Id, CancellationToken.None);

        user.AuthenticatorAppSecret.Should().Be("enc:v1:ciphertext-secret");
        user.IsAuthenticatorAppEnabled.Should().BeFalse();
        result.OtpauthUri.Should().StartWith("otpauth://totp/");
        result.QrPayload.Should().Be(result.OtpauthUri);
        result.SecretMasked.Should().NotBeNullOrWhiteSpace();

        authenticatorSecretProtector.Verify(x => x.Protect(It.Is<string>(s => !string.IsNullOrWhiteSpace(s))), Times.Once);
        userRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        userRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task VerifyAuthenticatorSetupAsync_WhenUnprotectFails_ShouldThrowRestartMessage()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "alice@example.com",
            IsActive = true,
            IsEmailVerified = true,
            AuthenticatorAppSecret = "enc:v1:broken"
        };

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var authenticatorSecretProtector = new Mock<IAuthenticatorSecretProtector>();
        var plaintext = string.Empty;
        authenticatorSecretProtector
            .Setup(x => x.TryUnprotect(user.AuthenticatorAppSecret, out plaintext))
            .Returns(false);

        var sut = CreateSut(
            userRepository,
            authenticatorSecretProtector,
            out _,
            out _);

        var act = () => sut.VerifyAuthenticatorSetupAsync(
            user.Id,
            new AuthenticatorSetupVerifyRequest { Code = "123456" },
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*restart setup*");

        userRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        userRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task VerifyLoginCodeAsync_AuthenticatorChannel_WhenUnprotectFails_ShouldIncrementAttemptsAndFail()
    {
        var challengeId = Guid.NewGuid();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "alice@example.com",
            IsActive = true,
            IsEmailVerified = true,
            LoginVerificationChallengeId = challengeId,
            LoginVerificationCodeExpiresAtUtc = DateTime.UtcNow.AddMinutes(5),
            LoginVerificationCodeAttempts = 0,
            LoginVerificationChannel = "authenticator_app",
            IsAuthenticatorAppEnabled = true,
            AuthenticatorAppSecret = "enc:v1:broken"
        };

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(x => x.GetActiveByEmailOrAccountNameWithAuthorizationAsync("alice@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        userRepository
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken _) => u);
        userRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var authenticatorSecretProtector = new Mock<IAuthenticatorSecretProtector>();
        var plaintext = string.Empty;
        authenticatorSecretProtector
            .Setup(x => x.TryUnprotect(user.AuthenticatorAppSecret, out plaintext))
            .Returns(false);

        var sut = CreateSut(
            userRepository,
            authenticatorSecretProtector,
            out _,
            out _);

        var act = () => sut.VerifyLoginCodeAsync(
            new LoginVerifyCodeRequest
            {
                Email = "alice@example.com",
                ChallengeId = challengeId,
                Code = "123456"
            },
            "127.0.0.1",
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*invalid or expired*");

        user.LoginVerificationCodeAttempts.Should().Be(1);
        userRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        userRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static AuthService CreateSut(
        Mock<IUserRepository> userRepository,
        Mock<IAuthenticatorSecretProtector> authenticatorSecretProtector,
        out Mock<IRefreshTokenRepository> refreshTokenRepository,
        out Mock<IJwtTokenGenerator> jwtTokenGenerator)
    {
        refreshTokenRepository = new Mock<IRefreshTokenRepository>();
        refreshTokenRepository
            .Setup(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken token, CancellationToken _) => token);
        refreshTokenRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var roleRepository = new Mock<IRoleRepository>();
        roleRepository
            .Setup(x => x.GetRoleIdByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var userRoleRepository = new Mock<IUserRoleRepository>();
        userRoleRepository
            .Setup(x => x.GetRoleNamesByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "booking-user" });
        userRoleRepository
            .Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var rolePermissionRepository = new Mock<IRolePermissionRepository>();
        rolePermissionRepository
            .Setup(x => x.GetPermissionNamesByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "appointment:view" });

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher
            .Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);
        passwordHasher
            .Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns("hashed");

        jwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        jwtTokenGenerator
            .Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<IReadOnlyCollection<string>>()))
            .Returns("access-token");

        return new AuthService(
            userRepository.Object,
            refreshTokenRepository.Object,
            roleRepository.Object,
            userRoleRepository.Object,
            rolePermissionRepository.Object,
            passwordHasher.Object,
            jwtTokenGenerator.Object,
            authenticatorSecretProtector.Object,
            new JwtOptions
            {
                Issuer = "https://auth.vehicle-service-booking.local",
                Audience = "vehicle-booking-api",
                AccessTokenLifetimeMinutes = 30,
                RefreshTokenLifetimeDays = 30
            });
    }
}
