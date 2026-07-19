namespace VehicleServiceBooking.Auth.Models;

/// <summary>
/// Join entity for user-to-group assignments.
/// </summary>
public class UserGroup : AuthBaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;
}
