namespace VehicleServiceBooking.Auth.Configuration;

/// <summary>
/// Configuration values for Google social login integration.
/// </summary>
public sealed class GoogleAuthOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether Google authentication is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets Google OAuth client identifier.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Google OAuth client secret.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets callback path handled by Google authentication middleware.
    /// </summary>
    public string CallbackPath { get; set; } = "/signin-google";

    /// <summary>
    /// Gets the temporary external sign-in cookie scheme.
    /// </summary>
    public const string ExternalCookieScheme = "GoogleExternal";
}