namespace VehicleServiceBooking.Auth.Models.Responses;

/// <summary>
/// Represents sign-up completion result when email verification is required.
/// </summary>
public sealed class SignUpResult
{
    public string Email { get; set; } = string.Empty;

    public string VerificationToken { get; set; } = string.Empty;

    public DateTime VerificationTokenExpiresAtUtc { get; set; }
}