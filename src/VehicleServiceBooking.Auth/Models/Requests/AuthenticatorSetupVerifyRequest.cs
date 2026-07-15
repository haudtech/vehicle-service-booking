using System.ComponentModel.DataAnnotations;

namespace VehicleServiceBooking.Auth.Models.Requests;

/// <summary>
/// Request payload to confirm authenticator-app setup by submitting a current TOTP code.
/// </summary>
public sealed class AuthenticatorSetupVerifyRequest
{
    [Required]
    [MinLength(6)]
    [MaxLength(8)]
    public string Code { get; set; } = string.Empty;
}
