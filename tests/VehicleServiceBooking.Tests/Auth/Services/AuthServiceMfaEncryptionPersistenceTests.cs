using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VehicleServiceBooking.Auth.Configuration;
using VehicleServiceBooking.Auth.Data;
using VehicleServiceBooking.Auth.Models;
using VehicleServiceBooking.Auth.Repositories;
using VehicleServiceBooking.Auth.Repositories.Interfaces;
using VehicleServiceBooking.Auth.Services;

namespace VehicleServiceBooking.Tests.Auth.Services;

public class AuthServiceMfaEncryptionPersistenceTests
{
    [Fact]
    public async Task StartAuthenticatorSetupAsync_ShouldPersistEncryptedSecretInDatabase()
    {
        var dbOptions = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase($"AuthServiceMfaEncryptionPersistenceTests_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new AuthDbContext(dbOptions);

        var userId = Guid.NewGuid();
        dbContext.Users.Add(new User
        {
            Id = userId,
            Email = "alice@example.com",
            AccountName = "alice",
            PasswordHash = "hashed",
            SecurityStamp = Guid.NewGuid().ToString("N"),
            DisplayName = "Alice",
            IsActive = true,
            IsEmailVerified = true,
            AuthenticatorAppSecret = string.Empty
        });
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var userRepository = new UserRepository(dbContext);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDataProtection();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var protector = new DataProtectionAuthenticatorSecretProtector(
            serviceProvider.GetRequiredService<Microsoft.AspNetCore.DataProtection.IDataProtectionProvider>());

        var sut = new AuthService(
            userRepository,
            Mock.Of<IRefreshTokenRepository>(),
            Mock.Of<IGroupRepository>(),
            Mock.Of<IUserGroupRepository>(),
            Mock.Of<IRolePermissionRepository>(),
            Mock.Of<IPasswordHasher>(),
            Mock.Of<IJwtTokenGenerator>(),
            protector,
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

        _ = await sut.StartAuthenticatorSetupAsync(userId, CancellationToken.None);

        var persisted = await dbContext.Users.AsNoTracking().FirstAsync(x => x.Id == userId);

        persisted.AuthenticatorAppSecret.Should().StartWith("enc:v1:");
        persisted.AuthenticatorAppSecret.Should().NotBeEmpty();
        persisted.IsAuthenticatorAppEnabled.Should().BeFalse();

        var ok = protector.TryUnprotect(persisted.AuthenticatorAppSecret, out var plaintextSecret);
        ok.Should().BeTrue();
        plaintextSecret.Should().NotBeNullOrWhiteSpace();
        plaintextSecret.Should().NotStartWith("enc:v1:");
    }
}
