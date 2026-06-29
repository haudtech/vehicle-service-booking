using System.Collections.Generic;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Static lookup table for idempotency request statuses.
/// </summary>
public class IdempotencyRequestStatusLookup : BaseEntity
{
    /// <summary>
    /// Enum value used by the application domain to represent this lookup status.
    /// </summary>
    public IdempotencyRequestStatus Status { get; set; }

    /// <summary>
    /// Display name of the status.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable description that explains the meaning of the status.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Idempotency requests currently associated with this status.
    /// </summary>
    public ICollection<IdempotencyRequest> IdempotencyRequests { get; set; } = new List<IdempotencyRequest>();
}
