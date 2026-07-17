namespace VehicleServiceBooking.Auth.Models.Responses;

/// <summary>
/// Core user profile used for internal cross-service bootstrap.
/// </summary>
public sealed class UserCoreProfileResponse
{
    public Guid AuthUserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsEmailVerified { get; set; }

    public bool IsActive { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}