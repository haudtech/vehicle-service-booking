namespace VehicleServiceBooking.Domain.Enums;

/// <summary>
/// Represents the processing lifecycle state of an idempotent request.
/// </summary>
public enum IdempotencyRequestStatus
{
    /// <summary>
    /// The request has been accepted and is currently being processed.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// The request has finished processing and the response is available for replay.
    /// </summary>
    Completed = 2
}
