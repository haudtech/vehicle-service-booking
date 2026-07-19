namespace VehicleServiceBooking.Auth.Models;

/// <summary>
/// Represents a user group for grouping and access semantics.
/// </summary>
public class Group : AuthBaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    public ICollection<GroupRole> GroupRoles { get; set; } = new List<GroupRole>();
}
