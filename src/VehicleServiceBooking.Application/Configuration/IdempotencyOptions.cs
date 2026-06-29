namespace VehicleServiceBooking.Application.Configuration;

/// <summary>
/// Configuration settings for request idempotency behavior on booking create endpoints.
/// Bound from the <c>Idempotency</c> configuration section.
/// </summary>
public class IdempotencyOptions
{
    /// <summary>
    /// Configuration section name used for options binding.
    /// </summary>
    public const string SectionName = "Idempotency";

    /// <summary>
    /// Enables or disables idempotency handling globally.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// When <see langword="true"/>, requests must include an <c>Idempotency-Key</c> header.
    /// </summary>
    public bool RequireHeader { get; set; } = false;

    /// <summary>
    /// Maximum allowed length of the idempotency key header value.
    /// </summary>
    public int KeyMaxLength { get; set; } = 100;

    /// <summary>
    /// Retention window, in hours, for stored idempotency entries.
    /// </summary>
    public int EntryTtlHours { get; set; } = 24;
}
