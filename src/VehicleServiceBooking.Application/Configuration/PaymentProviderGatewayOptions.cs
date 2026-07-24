namespace VehicleServiceBooking.Application.Configuration;

/// <summary>
/// Configuration for selecting and operating payment provider gateways.
/// </summary>
public sealed class PaymentProviderGatewayOptions
{
    public const string SectionName = "PaymentProviderGateway";

    /// <summary>
    /// When true, development gateway can be used as a fallback for providers/methods
    /// that do not yet have a real integration implementation.
    /// </summary>
    public bool UseDevelopmentGatewayFallback { get; set; } = true;

    public ZaloPayGatewayOptions ZaloPay { get; set; } = new();
}

/// <summary>
/// Real ZaloPay gateway settings for create-intent integration.
/// </summary>
public sealed class ZaloPayGatewayOptions
{
    public bool Enabled { get; set; }

    public string BaseUrl { get; set; } = string.Empty;

    public string CreateOrderPath { get; set; } = "/v2/create";

    public string QueryOrderPath { get; set; } = "/v2/query";

    public int AppId { get; set; }

    public string Key1 { get; set; } = string.Empty;

    public string Key2 { get; set; } = string.Empty;

    public string AppUserPrefix { get; set; } = "vsb";

    public string CallbackUrl { get; set; } = string.Empty;

    public string RedirectUrl { get; set; } = string.Empty;

    public int IntentTtlMinutes { get; set; } = 15;

    public int RequestTimeoutSeconds { get; set; } = 15;
}
