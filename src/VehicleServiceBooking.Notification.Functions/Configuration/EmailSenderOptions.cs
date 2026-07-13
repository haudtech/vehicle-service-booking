namespace VehicleServiceBooking.Notification.Functions.Configuration;

/// <summary>
/// Configures email sender behavior for notification delivery.
/// </summary>
public sealed class EmailSenderOptions
{
    /// <summary>
    /// Gets or sets logical provider name (for example Logging, SendGrid, or Google).
    /// </summary>
    public string Provider { get; set; } = "Logging";

    /// <summary>
    /// Gets or sets sender address used in outbound emails.
    /// </summary>
    public string FromAddress { get; set; } = "noreply@vehicle-service-booking.local";

    /// <summary>
    /// Gets or sets sender display name used in outbound emails.
    /// </summary>
    public string FromName { get; set; } = "Vehicle Service Booking";

    /// <summary>
    /// Gets or sets SendGrid API key when provider is SendGrid.
    /// </summary>
    public string SendGridApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets SendGrid API endpoint URL.
    /// </summary>
    public string SendGridEndpoint { get; set; } = "https://api.sendgrid.com/v3/mail/send";

    /// <summary>
    /// Gets or sets Gmail API base endpoint.
    /// </summary>
    public string GoogleApiEndpoint { get; set; } = "https://gmail.googleapis.com/gmail/v1/users";

    /// <summary>
    /// Gets or sets OAuth token endpoint for Google.
    /// </summary>
    public string GoogleTokenEndpoint { get; set; } = "https://oauth2.googleapis.com/token";

    /// <summary>
    /// Gets or sets Gmail user id. Use "me" for authenticated user.
    /// </summary>
    public string GoogleUserId { get; set; } = "me";

    /// <summary>
    /// Gets or sets Google OAuth client id.
    /// </summary>
    public string GoogleClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Google OAuth client secret.
    /// </summary>
    public string GoogleClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Google OAuth refresh token for automatic access token retrieval.
    /// </summary>
    public string GoogleRefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional pre-issued Google OAuth access token.
    /// </summary>
    public string GoogleAccessToken { get; set; } = string.Empty;
}
