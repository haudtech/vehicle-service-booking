namespace VehicleServiceBooking.Auth.Models;

/// <summary>
/// Represents a role that grants permissions to users.
/// </summary>
public class Role : AuthBaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<GroupRole> GroupRoles { get; set; } = new List<GroupRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
