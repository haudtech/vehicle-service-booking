using System.ComponentModel.DataAnnotations;

namespace VehicleServiceBooking.Auth.Models.Requests;

/// <summary>
/// Request payload for user registration.
/// </summary>
public sealed class SignUpRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string AccountName { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;
}
