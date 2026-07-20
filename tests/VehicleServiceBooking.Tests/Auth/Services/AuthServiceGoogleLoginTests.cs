using FluentAssertions;
using Moq;
using VehicleServiceBooking.Auth.Configuration;
using VehicleServiceBooking.Auth.Models;
using VehicleServiceBooking.Auth.Models.Requests;
using VehicleServiceBooking.Auth.Repositories.Interfaces;
using VehicleServiceBooking.Auth.Services;

namespace VehicleServiceBooking.Tests.Auth.Services;

public class AuthServiceGoogleLoginTests
{
    [Fact]
    public async Task LoginWithGoogleAsync_SameEmailCalledTwice_ProvisionsOnceThenReusesUser()
    {
        User? storedUser = null;

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string normalizedEmail, CancellationToken _) =>
                storedUser is not null && storedUser.Email == normalizedEmail ? storedUser : null);
        userRepository
            .Setup(x => x.GetByAccountNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        userRepository
            .Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken _) =>
            {
                storedUser = user;
                return user;
            });
        userRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var refreshTokenRepository = new Mock<IRefreshTokenRepository>();
        refreshTokenRepository
            .Setup(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken token, CancellationToken _) => token);
        refreshTokenRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var groupId = Guid.NewGuid();
        var groupRepository = new Mock<IGroupRepository>();
        groupRepository
            .Setup(x => x.GetGroupIdByNameAsync("user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(groupId);

        var userGroupRepository = new Mock<IUserGroupRepository>();
        userGroupRepository
            .Setup(x => x.ExistsAsync(It.IsAny<Guid>(), groupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        userGroupRepository
            .Setup(x => x.AddAsync(It.IsAny<UserGroup>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserGroup mapping, CancellationToken _) => mapping);
        userGroupRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        userGroupRepository
            .Setup(x => x.GetRoleNamesByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "booking-user" });

        var rolePermissionRepository = new Mock<IRolePermissionRepository>();
        rolePermissionRepository
            .Setup(x => x.GetPermissionNamesByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "appointment:view" });

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher
            .Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns("hashed");

        var jwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        jwtTokenGenerator
            .Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<IReadOnlyCollection<string>>()))
            .Returns("access-token");

        var authenticatorSecretProtector = new Mock<IAuthenticatorSecretProtector>();
        authenticatorSecretProtector
            .Setup(x => x.Protect(It.IsAny<string>()))
            .Returns((string s) => s);
        authenticatorSecretProtector
            .Setup(x => x.TryUnprotect(It.IsAny<string>(), out It.Ref<string>.IsAny))
            .Returns((string stored, out string plaintext) =>
            {
                plaintext = stored;
                return true;
            });

        var sut = new AuthService(
            userRepository.Object,
            refreshTokenRepository.Object,
            groupRepository.Object,
            userGroupRepository.Object,
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
            },
            new AuthDefaultGroupsOptions
            {
                DefaultSignupGroupNames = new List<string> { "user" }
            });

        var first = await sut.LoginWithGoogleAsync(
            "google.user@example.com",
            "Google User",
            "google-subject-123",
            true,
            "127.0.0.1",
            CancellationToken.None);

        var second = await sut.LoginWithGoogleAsync(
            "google.user@example.com",
            "Google User",
            "google-subject-123",
            true,
            "127.0.0.1",
            CancellationToken.None);

        first.AccessToken.Should().Be("access-token");
        second.AccessToken.Should().Be("access-token");

        userRepository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        userRepository.Verify(x => x.GetByEmailAsync("google.user@example.com", It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
