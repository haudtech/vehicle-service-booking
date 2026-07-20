namespace VehicleServiceBooking.Auth.Models;

/// <summary>
/// Join entity for group-to-role assignments.
/// </summary>
public class GroupRole : AuthBaseEntity
{
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
