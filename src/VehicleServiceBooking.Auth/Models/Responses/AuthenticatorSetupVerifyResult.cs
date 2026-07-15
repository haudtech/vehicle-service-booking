namespace VehicleServiceBooking.Auth.Models.Responses;

/// <summary>
/// Result returned when authenticator-app setup is successfully verified.
/// </summary>
public sealed class AuthenticatorSetupVerifyResult
{
    /// <summary>
    /// Indicates whether authenticator app MFA is now enabled for the user.
    /// </summary>
    public bool IsAuthenticatorAppEnabled { get; set; }

    /// <summary>
    /// Active login challenge channel preference stored for the user.
    /// </summary>
    public string LoginVerificationChannel { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable purpose statement for completion state.
    /// </summary>
    public string Purpose { get; set; } = "Authenticator app is verified and can now be used for login challenges.";
}
