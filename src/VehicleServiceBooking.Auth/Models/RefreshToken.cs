namespace VehicleServiceBooking.Auth.Models;

/// <summary>
/// Represents a persisted refresh token for token renewal.
/// </summary>
public class RefreshToken : AuthBaseEntity
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string CreatedByIp { get; set; } = string.Empty;

    public bool IsUsable => IsActive && RevokedAt == null && DateTime.UtcNow < ExpiresAt;
}
