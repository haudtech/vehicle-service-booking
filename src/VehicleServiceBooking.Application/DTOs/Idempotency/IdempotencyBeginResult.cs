using System;

namespace VehicleServiceBooking.Application.DTOs;

/// <summary>
/// Represents the result of attempting to begin idempotent request processing.
/// </summary>
public sealed class IdempotencyBeginResult
{
    /// <summary>
    /// Describes whether processing can continue, should wait, or should replay a stored response.
    /// </summary>
    public IdempotencyBeginOutcome Outcome { get; init; }

    /// <summary>
    /// Identifier of the idempotency record when one is created or found; otherwise <see langword="null"/>.
    /// </summary>
    public Guid? RecordId { get; init; }

    /// <summary>
    /// HTTP status code of a previously persisted response when replay is required; otherwise <see langword="null"/>.
    /// </summary>
    public int? ResponseStatusCode { get; init; }

    /// <summary>
    /// Serialized response body of a previously persisted response when replay is required; otherwise <see langword="null"/>.
    /// </summary>
    public string? ResponseBody { get; init; }
}