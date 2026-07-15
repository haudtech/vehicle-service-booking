namespace VehicleServiceBooking.Auth.Models.Responses;

/// <summary>
/// Result returned when authenticator-app setup is initialized.
/// </summary>
public sealed class AuthenticatorSetupStartResult
{
    /// <summary>
    /// Full otpauth URI containing enrollment metadata and shared secret.
    /// </summary>
    public string OtpauthUri { get; set; } = string.Empty;

    /// <summary>
    /// Raw QR payload. For most clients this is the same value as <see cref="OtpauthUri"/>.
    /// </summary>
    public string QrPayload { get; set; } = string.Empty;

    /// <summary>
    /// Authorized endpoint that returns a PNG QR image for authenticator enrollment.
    /// </summary>
    public string QrImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Partially masked secret for user confirmation/debug display.
    /// </summary>
    public string SecretMasked { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable purpose statement for the client flow.
    /// </summary>
    public string Purpose { get; set; } = "Scan QR in authenticator app, then verify with a current code.";
}
