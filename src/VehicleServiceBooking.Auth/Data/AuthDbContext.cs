using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Auth.Common.Enums;
using VehicleServiceBooking.Auth.Models;

namespace VehicleServiceBooking.Auth.Data;

/// <summary>
/// Entity Framework DbContext for auth service persistence.
/// </summary>
public class AuthDbContext : DbContext
{
    private static readonly DateTime SeedTimestampUtc = new(2026, 7, 4, 0, 0, 0, DateTimeKind.Utc);
    private static readonly Guid BookingAdminRoleId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5A011");
    private static readonly Guid BookingUserRoleId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5A001");
    private static readonly Guid OrderAdminRoleId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5A012");
    private static readonly Guid OrderUserRoleId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5A010");

    private static readonly Guid UserGroupId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5C001");
    private static readonly Guid AdminGroupId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5C002");
    private static readonly Guid SuperAdminGroupId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5C003");

    private static readonly Guid AppointmentCreatePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5A002");
    private static readonly Guid AppointmentViewPermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5A003");
    private static readonly Guid AppointmentCompletePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5A004");
    private static readonly Guid OrderCreatePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5A005");
    private static readonly Guid OrderViewPermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5A006");
    private static readonly Guid OrderEditPermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5A007");
    private static readonly Guid OrderCancelPermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5A008");
    private static readonly Guid OrderCompletePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5A009");
    private static readonly Guid BookingUserAppointmentCreateRolePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5B001");
    private static readonly Guid BookingUserAppointmentViewRolePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5B002");
    private static readonly Guid BookingUserAppointmentCompleteRolePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5B003");
    private static readonly Guid OrderUserOrderCreateRolePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5B005");
    private static readonly Guid OrderUserOrderViewRolePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5B006");
    private static readonly Guid OrderUserOrderEditRolePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5B007");
    private static readonly Guid OrderUserOrderCancelRolePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5B008");
    private static readonly Guid OrderUserOrderCompleteRolePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5B009");

    private static readonly Guid BookingAdminAppointmentCreateRolePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5B011");
    private static readonly Guid BookingAdminAppointmentViewRolePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5B012");
    private static readonly Guid BookingAdminAppointmentCompleteRolePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5B013");
    private static readonly Guid OrderAdminOrderCreateRolePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5B014");
    private static readonly Guid OrderAdminOrderViewRolePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5B015");
    private static readonly Guid OrderAdminOrderEditRolePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5B016");
    private static readonly Guid OrderAdminOrderCancelRolePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5B017");
    private static readonly Guid OrderAdminOrderCompleteRolePermissionId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5B018");

    private static readonly Guid UserGroupBookingUserGroupRoleId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5D001");
    private static readonly Guid UserGroupOrderUserGroupRoleId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5D002");
    private static readonly Guid AdminGroupBookingAdminGroupRoleId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5D003");
    private static readonly Guid AdminGroupOrderAdminGroupRoleId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5D004");
    private static readonly Guid SuperAdminGroupBookingAdminGroupRoleId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5D005");
    private static readonly Guid SuperAdminGroupBookingUserGroupRoleId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5D006");
    private static readonly Guid SuperAdminGroupOrderAdminGroupRoleId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5D007");
    private static readonly Guid SuperAdminGroupOrderUserGroupRoleId = Guid.Parse("2D7B6113-2351-4B60-848F-8C3D28F5D008");

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
    /// Gets user-group mappings.
    /// </summary>
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();

    /// <summary>
    /// Gets group-role mappings.
    /// </summary>
    public DbSet<GroupRole> GroupRoles => Set<GroupRole>();

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
            builder.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(20).HasDefaultValue(string.Empty);
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

            builder.HasData(
                new Role
                {
                    Id = BookingAdminRoleId,
                    Name = "booking-admin",
                    Description = "Administrator role for booking resource operations",
                    IsActive = true,
                    CreatedAt = SeedTimestampUtc,
                    UpdatedAt = SeedTimestampUtc
                },
                new Role
                {
                    Id = BookingUserRoleId,
                    Name = "booking-user",
                    Description = "Default role for booking resource operations",
                    IsActive = true,
                    CreatedAt = SeedTimestampUtc,
                    UpdatedAt = SeedTimestampUtc
                },
                new Role
                {
                    Id = OrderAdminRoleId,
                    Name = "order-admin",
                    Description = "Administrator role for order resource operations",
                    IsActive = true,
                    CreatedAt = SeedTimestampUtc,
                    UpdatedAt = SeedTimestampUtc
                },
                new Role
                {
                    Id = OrderUserRoleId,
                    Name = "order-user",
                    Description = "Default role for order resource operations",
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

            builder.HasData(
                new Group
                {
                    Id = UserGroupId,
                    Name = "user",
                    Description = "Default group for standard end users",
                    IsActive = true,
                    CreatedAt = SeedTimestampUtc,
                    UpdatedAt = SeedTimestampUtc
                },
                new Group
                {
                    Id = AdminGroupId,
                    Name = "admin",
                    Description = "Administrative group",
                    IsActive = true,
                    CreatedAt = SeedTimestampUtc,
                    UpdatedAt = SeedTimestampUtc
                },
                new Group
                {
                    Id = SuperAdminGroupId,
                    Name = "superadmin",
                    Description = "Super administrator group with full role coverage",
                    IsActive = true,
                    CreatedAt = SeedTimestampUtc,
                    UpdatedAt = SeedTimestampUtc
                });
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
                },
                new Permission
                {
                    Id = OrderCreatePermissionId,
                    Name = "order:create",
                    Description = "Create orders",
                    IsActive = true,
                    CreatedAt = SeedTimestampUtc,
                    UpdatedAt = SeedTimestampUtc
                },
                new Permission
                {
                    Id = OrderViewPermissionId,
                    Name = "order:view",
                    Description = "View orders",
                    IsActive = true,
                    CreatedAt = SeedTimestampUtc,
                    UpdatedAt = SeedTimestampUtc
                },
                new Permission
                {
                    Id = OrderEditPermissionId,
                    Name = "order:edit",
                    Description = "Edit orders",
                    IsActive = true,
                    CreatedAt = SeedTimestampUtc,
                    UpdatedAt = SeedTimestampUtc
                },
                new Permission
                {
                    Id = OrderCancelPermissionId,
                    Name = "order:cancel",
                    Description = "Cancel orders",
                    IsActive = true,
                    CreatedAt = SeedTimestampUtc,
                    UpdatedAt = SeedTimestampUtc
                },
                new Permission
                {
                    Id = OrderCompletePermissionId,
                    Name = "order:complete",
                    Description = "Complete orders",
                    IsActive = true,
                    CreatedAt = SeedTimestampUtc,
                    UpdatedAt = SeedTimestampUtc
                });
        });

        modelBuilder.Entity<UserGroup>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.UserId, x.GroupId }).IsUnique();
            builder.HasOne(x => x.User).WithMany(x => x.UserGroups).HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.Group).WithMany(x => x.UserGroups).HasForeignKey(x => x.GroupId);
            builder.HasQueryFilter(x => x.IsActive && x.User.IsActive && x.Group.IsActive);
        });

        modelBuilder.Entity<GroupRole>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.GroupId, x.RoleId }).IsUnique();
            builder.HasOne(x => x.Group).WithMany(x => x.GroupRoles).HasForeignKey(x => x.GroupId);
            builder.HasOne(x => x.Role).WithMany(x => x.GroupRoles).HasForeignKey(x => x.RoleId);
            builder.HasQueryFilter(x => x.IsActive && x.Group.IsActive && x.Role.IsActive);

            builder.HasData(
                new GroupRole { Id = UserGroupBookingUserGroupRoleId, GroupId = UserGroupId, RoleId = BookingUserRoleId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new GroupRole { Id = UserGroupOrderUserGroupRoleId, GroupId = UserGroupId, RoleId = OrderUserRoleId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new GroupRole { Id = AdminGroupBookingAdminGroupRoleId, GroupId = AdminGroupId, RoleId = BookingAdminRoleId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new GroupRole { Id = AdminGroupOrderAdminGroupRoleId, GroupId = AdminGroupId, RoleId = OrderAdminRoleId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new GroupRole { Id = SuperAdminGroupBookingAdminGroupRoleId, GroupId = SuperAdminGroupId, RoleId = BookingAdminRoleId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new GroupRole { Id = SuperAdminGroupBookingUserGroupRoleId, GroupId = SuperAdminGroupId, RoleId = BookingUserRoleId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new GroupRole { Id = SuperAdminGroupOrderAdminGroupRoleId, GroupId = SuperAdminGroupId, RoleId = OrderAdminRoleId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new GroupRole { Id = SuperAdminGroupOrderUserGroupRoleId, GroupId = SuperAdminGroupId, RoleId = OrderUserRoleId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc });
        });

        modelBuilder.Entity<RolePermission>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.RoleId, x.PermissionId }).IsUnique();
            builder.HasOne(x => x.Role).WithMany(x => x.RolePermissions).HasForeignKey(x => x.RoleId);
            builder.HasOne(x => x.Permission).WithMany(x => x.RolePermissions).HasForeignKey(x => x.PermissionId);
            builder.HasQueryFilter(x => x.IsActive && x.Role.IsActive && x.Permission.IsActive);

            builder.HasData(
                new RolePermission { Id = BookingUserAppointmentCreateRolePermissionId, RoleId = BookingUserRoleId, PermissionId = AppointmentCreatePermissionId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new RolePermission { Id = BookingUserAppointmentViewRolePermissionId, RoleId = BookingUserRoleId, PermissionId = AppointmentViewPermissionId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new RolePermission { Id = BookingUserAppointmentCompleteRolePermissionId, RoleId = BookingUserRoleId, PermissionId = AppointmentCompletePermissionId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new RolePermission { Id = OrderUserOrderCreateRolePermissionId, RoleId = OrderUserRoleId, PermissionId = OrderCreatePermissionId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new RolePermission { Id = OrderUserOrderViewRolePermissionId, RoleId = OrderUserRoleId, PermissionId = OrderViewPermissionId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new RolePermission { Id = OrderUserOrderEditRolePermissionId, RoleId = OrderUserRoleId, PermissionId = OrderEditPermissionId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new RolePermission { Id = OrderUserOrderCancelRolePermissionId, RoleId = OrderUserRoleId, PermissionId = OrderCancelPermissionId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new RolePermission { Id = OrderUserOrderCompleteRolePermissionId, RoleId = OrderUserRoleId, PermissionId = OrderCompletePermissionId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new RolePermission { Id = BookingAdminAppointmentCreateRolePermissionId, RoleId = BookingAdminRoleId, PermissionId = AppointmentCreatePermissionId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new RolePermission { Id = BookingAdminAppointmentViewRolePermissionId, RoleId = BookingAdminRoleId, PermissionId = AppointmentViewPermissionId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new RolePermission { Id = BookingAdminAppointmentCompleteRolePermissionId, RoleId = BookingAdminRoleId, PermissionId = AppointmentCompletePermissionId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new RolePermission { Id = OrderAdminOrderCreateRolePermissionId, RoleId = OrderAdminRoleId, PermissionId = OrderCreatePermissionId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new RolePermission { Id = OrderAdminOrderViewRolePermissionId, RoleId = OrderAdminRoleId, PermissionId = OrderViewPermissionId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new RolePermission { Id = OrderAdminOrderEditRolePermissionId, RoleId = OrderAdminRoleId, PermissionId = OrderEditPermissionId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new RolePermission { Id = OrderAdminOrderCancelRolePermissionId, RoleId = OrderAdminRoleId, PermissionId = OrderCancelPermissionId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc },
                new RolePermission { Id = OrderAdminOrderCompleteRolePermissionId, RoleId = OrderAdminRoleId, PermissionId = OrderCompletePermissionId, IsActive = true, CreatedAt = SeedTimestampUtc, UpdatedAt = SeedTimestampUtc });
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
