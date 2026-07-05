namespace VehicleServiceBooking.Auth.Models;

/// <summary>
/// Represents a role that grants permissions to users.
/// </summary>
public class Role : AuthBaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
