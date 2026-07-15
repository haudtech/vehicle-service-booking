using System.ComponentModel.DataAnnotations;
using VehicleServiceBooking.Auth.Common.Enums;

namespace VehicleServiceBooking.Auth.Models.Requests;

/// <summary>
/// Request payload for user login.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// Requested challenge channel.
    /// Supported values: otp_first, email_otp, authenticator_app.
    /// </summary>
    [MaxLength(32)]
    public string ChallengeChannel { get; set; } = Common.Enums.ChallengeChannel.OtpFirst.ToWireValue();

    [MaxLength(256)]
    public string Identifier { get; set; } = string.Empty;

    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
