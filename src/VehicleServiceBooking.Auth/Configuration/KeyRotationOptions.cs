namespace VehicleServiceBooking.Auth.Configuration;

/// <summary>
/// Configurable options controlling in-memory JWT signing key rotation behavior.
/// </summary>
public sealed class KeyRotationOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether runtime key rotation is enabled.
    /// </summary>
    public bool EnableRotation { get; set; } = true;

    /// <summary>
    /// Gets or sets the overlap window (minutes) during which previous keys remain valid for verification.
    /// </summary>
    public int OverlapMinutes { get; set; } = 60;

    /// <summary>
    /// Gets or sets the maximum number of keys to publish in JWKS at any given time.
    /// </summary>
    public int MaxPublishedKeys { get; set; } = 2;
}