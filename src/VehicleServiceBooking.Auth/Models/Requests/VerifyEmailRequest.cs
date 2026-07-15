using System.ComponentModel.DataAnnotations;

namespace VehicleServiceBooking.Auth.Models.Requests;

/// <summary>
/// Request payload for confirming email ownership.
/// </summary>
public sealed class VerifyEmailRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(16)]
    [MaxLength(512)]
    public string Token { get; set; } = string.Empty;
}