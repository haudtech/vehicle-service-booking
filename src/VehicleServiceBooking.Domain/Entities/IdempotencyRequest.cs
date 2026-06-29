namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Stores request metadata and response replay data used to enforce idempotent API behavior.
/// </summary>
public class IdempotencyRequest : BaseEntity
{
    /// <summary>
    /// Client-provided idempotency key used to identify a logical request.
    /// </summary>
    public string IdempotencyKey { get; set; } = string.Empty;

    /// <summary>
    /// Normalized request path associated with the idempotency key.
    /// </summary>
    public string RequestPath { get; set; } = string.Empty;

    /// <summary>
    /// Hash of the request payload used to detect key reuse with different content.
    /// </summary>
    public string RequestHash { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to the static lookup status row for this idempotency record.
    /// </summary>
    public Guid StatusId { get; set; }

    /// <summary>
    /// Navigation property to the current processing status lookup value.
    /// </summary>
    public IdempotencyRequestStatusLookup Status { get; set; } = null!;

    /// <summary>
    /// HTTP status code persisted after successful processing for replay scenarios.
    /// </summary>
    public int? ResponseStatusCode { get; set; }

    /// <summary>
    /// Serialized response body persisted for replay when duplicate requests are received.
    /// </summary>
    public string? ResponseBody { get; set; }

    /// <summary>
    /// UTC timestamp when this idempotency record expires and may be cleaned up.
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
