using System.ComponentModel.DataAnnotations;

namespace VehicleServiceBooking.Auth.Models.Requests;

/// <summary>
/// Request payload for refresh token exchange and revocation operations.
/// </summary>
public sealed class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
