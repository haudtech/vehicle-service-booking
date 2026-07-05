namespace VehicleServiceBooking.Auth.Models;

/// <summary>
/// Join entity for role-to-permission assignments.
/// </summary>
public class RolePermission
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}
