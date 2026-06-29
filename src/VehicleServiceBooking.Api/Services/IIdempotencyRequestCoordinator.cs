using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleServiceBooking.Application.DTOs;

namespace VehicleServiceBooking.Api.Services;

/// <summary>
/// Coordinates API-layer idempotency validation and begin-request flow for write endpoints.
/// </summary>
public interface IIdempotencyRequestCoordinator
{
    /// <summary>
    /// Validates idempotency header requirements and starts idempotent processing for appointment creation.
    /// </summary>
    Task<IdempotencyCoordinatorResult> ValidateAndBeginCreateAppointmentAsync(
        HttpRequest httpRequest,
        CreateAppointmentRequest request,
        CancellationToken cancellationToken);
}

/// <summary>
/// Represents the coordinator outcome for idempotency handling in API endpoints.
/// </summary>
public sealed class IdempotencyCoordinatorResult
{
    /// <summary>
    /// Early response that should be returned immediately, or <see langword="null"/> when processing can continue.
    /// </summary>
    public ActionResult? EarlyResponse { get; init; }

    /// <summary>
    /// Idempotency record identifier to use for completion/release, or <see langword="null"/> when none is active.
    /// </summary>
    public Guid? RecordId { get; init; }
}