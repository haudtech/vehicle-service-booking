namespace VehicleServiceBooking.Auth.Models;

/// <summary>
/// Represents a named permission assignable to roles.
/// </summary>
public class Permission : AuthBaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
