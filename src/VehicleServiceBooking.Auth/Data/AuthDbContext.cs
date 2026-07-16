using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Auth.Models;
using VehicleServiceBooking.Auth.Common.Enums;

namespace VehicleServiceBooking.Auth.Data;

/// <summary>
/// Entity Framework DbContext for auth service persistence.
/// </summary>
public class AuthDbContext : DbContext
{
    private static readonly DateTime SeedTimestampUtc = new(2026, 7, 4, 0, 0, 0, DateTimeKind.Utc);
    private static readonly Guid BookingUserRoleId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5A001");
    private static readonly Guid AppointmentCreatePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5A002");
    private static readonly Guid AppointmentViewPermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5A003");
    private static readonly Guid AppointmentCompletePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5A004");

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthDbContext"/> class.
    /// </summary>
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets users.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Gets roles.
    /// </summary>
    public DbSet<Role> Roles => Set<Role>();

    /// <summary>
    /// Gets groups.
    /// </summary>
    public DbSet<Group> Groups => Set<Group>();

    /// <summary>
    /// Gets permissions.
    /// </summary>
    public DbSet<Permission> Permissions => Set<Permission>();

    /// <summary>
    /// Gets user-role mappings.
    /// </summary>
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    /// <summary>
    /// Gets user-group mappings.
    /// </summary>
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();

    /// <summary>
    /// Gets role-permission mappings.
    /// </summary>
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    /// <summary>
    /// Gets refresh tokens.
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var baseEntityType = typeof(AuthBaseEntity);
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!baseEntityType.IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            modelBuilder.Entity(entityType.ClrType)
                .Property("CreatedAt")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            modelBuilder.Entity(entityType.ClrType)
                .Property("UpdatedAt")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            modelBuilder.Entity(entityType.ClrType)
                .Property("IsActive")
                .HasDefaultValue(true)
                .IsRequired();

            // Warning context:
            // EF may warn when filtered principals (Role/Group/Permission) are required by join entities.
            // This is acceptable for now because we are not relying on soft-delete recovery behavior yet.
            // If soft-delete workflows are introduced for these principals, revisit join/filter strategy.
            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(CreateIsActiveFilter(entityType.ClrType));
        }

        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.AccountName).IsUnique();
            builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
            builder.Property(x => x.IsEmailVerified).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.EmailVerifiedAtUtc);
            builder.Property(x => x.EmailVerificationTokenHash).IsRequired().HasMaxLength(128).HasDefaultValue(string.Empty);
            builder.Property(x => x.EmailVerificationTokenExpiresAtUtc);
            builder.Property(x => x.LoginVerificationChallengeId);
            builder.Property(x => x.LoginVerificationCodeHash).IsRequired().HasMaxLength(128).HasDefaultValue(string.Empty);
            builder.Property(x => x.LoginVerificationCodeExpiresAtUtc);
            builder.Property(x => x.LoginVerificationCodeAttempts).IsRequired().HasDefaultValue(0);
            builder.Property(x => x.LoginVerificationChannel).IsRequired().HasMaxLength(32).HasDefaultValue(ChallengeChannel.EmailOtp.ToWireValue());
            builder.Property(x => x.IsAuthenticatorAppEnabled).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.AuthenticatorAppSecret).IsRequired().HasMaxLength(256).HasDefaultValue(string.Empty);
            builder.Property(x => x.ZaloUserId).IsRequired().HasMaxLength(128).HasDefaultValue(string.Empty);
            builder.Property(x => x.AccountName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.PasswordHash).IsRequired();
            builder.Property(x => x.SecurityStamp).IsRequired().HasMaxLength(50);
            builder.Property(x => x.DisplayName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.IsActive).IsRequired();
        });

        modelBuilder.Entity<Role>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Name).IsUnique();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).HasMaxLength(250);

            builder.HasData(new Role
            {
                Id = BookingUserRoleId,
                Name = "booking-user",
                Description = "Default role for booking service API access",
                IsActive = true,
                CreatedAt = SeedTimestampUtc,
                UpdatedAt = SeedTimestampUtc
            });
        });

        modelBuilder.Entity<Group>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Name).IsUnique();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).HasMaxLength(250);
        });

        modelBuilder.Entity<Permission>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Name).IsUnique();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Description).HasMaxLength(250);

            builder.HasData(
                new Permission
                {
                    Id = AppointmentCreatePermissionId,
                    Name = "appointment:create",
                    Description = "Create appointments",
                    IsActive = true,
                    CreatedAt = SeedTimestampUtc,
                    UpdatedAt = SeedTimestampUtc
                },
                new Permission
                {
                    Id = AppointmentViewPermissionId,
                    Name = "appointment:view",
                    Description = "View appointment availability and details",
                    IsActive = true,
                    CreatedAt = SeedTimestampUtc,
                    UpdatedAt = SeedTimestampUtc
                },
                new Permission
                {
                    Id = AppointmentCompletePermissionId,
                    Name = "appointment:complete",
                    Description = "Complete appointments",
                    IsActive = true,
                    CreatedAt = SeedTimestampUtc,
                    UpdatedAt = SeedTimestampUtc
                });
        });

        modelBuilder.Entity<UserRole>(builder =>
        {
            builder.HasKey(x => new { x.UserId, x.RoleId });
            builder.HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId);
            builder.HasQueryFilter(x => x.User.IsActive && x.Role.IsActive);
        });

        modelBuilder.Entity<UserGroup>(builder =>
        {
            builder.HasKey(x => new { x.UserId, x.GroupId });
            builder.HasOne(x => x.User).WithMany(x => x.UserGroups).HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.Group).WithMany(x => x.UserGroups).HasForeignKey(x => x.GroupId);
            builder.HasQueryFilter(x => x.User.IsActive && x.Group.IsActive);
        });

        modelBuilder.Entity<RolePermission>(builder =>
        {
            builder.HasKey(x => new { x.RoleId, x.PermissionId });
            builder.HasOne(x => x.Role).WithMany(x => x.RolePermissions).HasForeignKey(x => x.RoleId);
            builder.HasOne(x => x.Permission).WithMany(x => x.RolePermissions).HasForeignKey(x => x.PermissionId);
            builder.HasQueryFilter(x => x.Role.IsActive && x.Permission.IsActive);

            builder.HasData(
                new RolePermission { RoleId = BookingUserRoleId, PermissionId = AppointmentCreatePermissionId },
                new RolePermission { RoleId = BookingUserRoleId, PermissionId = AppointmentViewPermissionId },
                new RolePermission { RoleId = BookingUserRoleId, PermissionId = AppointmentCompletePermissionId });
        });

        modelBuilder.Entity<RefreshToken>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Token).IsRequired();
            builder.Property(x => x.ExpiresAt).IsRequired();
            builder.Property(x => x.CreatedByIp).IsRequired().HasMaxLength(50);
            builder.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId);
        });
    }

    private static LambdaExpression CreateIsActiveFilter(Type clrType)
    {
        var parameter = Expression.Parameter(clrType, "entity");
        var propertyAccess = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            new[] { typeof(bool) },
            parameter,
            Expression.Constant("IsActive"));
        var body = Expression.Equal(propertyAccess, Expression.Constant(true));
        return Expression.Lambda(body, parameter);
    }
}
