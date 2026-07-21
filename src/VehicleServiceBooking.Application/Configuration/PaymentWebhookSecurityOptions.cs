namespace VehicleServiceBooking.Application.Configuration;

/// <summary>
/// Configuration for payment webhook signature validation.
/// </summary>
public sealed class PaymentWebhookSecurityOptions
{
    public const string SectionName = "PaymentWebhookSecurity";

    public bool Enabled { get; set; } = true;

    public bool RequireSignature { get; set; } = true;

    public string SharedSecret { get; set; } = string.Empty;
}