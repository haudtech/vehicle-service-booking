namespace VehicleServiceBooking.Auth.Models;

/// <summary>
/// Represents an authenticated system user.
/// </summary>
public class User : AuthBaseEntity
{
    public string Email { get; set; } = string.Empty;

    public string AccountName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

    public string DisplayName { get; set; } = string.Empty;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
