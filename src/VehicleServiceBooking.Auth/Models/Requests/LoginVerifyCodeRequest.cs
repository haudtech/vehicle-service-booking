using System.ComponentModel.DataAnnotations;

namespace VehicleServiceBooking.Auth.Models.Requests;

/// <summary>
/// Request payload for completing a login challenge using an email verification code.
/// </summary>
public sealed class LoginVerifyCodeRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public Guid ChallengeId { get; set; }

    [Required]
    [MinLength(4)]
    [MaxLength(16)]
    public string Code { get; set; } = string.Empty;
}
