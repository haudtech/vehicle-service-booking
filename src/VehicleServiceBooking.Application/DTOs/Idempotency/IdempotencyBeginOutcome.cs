namespace VehicleServiceBooking.Application.DTOs;

/// <summary>
/// Describes the decision made when starting idempotent request processing.
/// </summary>
public enum IdempotencyBeginOutcome
{
    /// <summary>
    /// No conflicting request exists and processing should continue normally.
    /// </summary>
    Proceed,

    /// <summary>
    /// A matching completed request exists and its stored response should be returned.
    /// </summary>
    ReplayStoredResponse,

    /// <summary>
    /// The idempotency key was reused with a different payload and must be rejected.
    /// </summary>
    KeyReusedWithDifferentPayload,

    /// <summary>
    /// A matching request is currently being processed and the caller should retry later.
    /// </summary>
    RequestInProgress
}