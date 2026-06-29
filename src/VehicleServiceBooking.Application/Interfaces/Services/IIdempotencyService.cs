using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Application.DTOs;

namespace VehicleServiceBooking.Application.Interfaces.Services;

public interface IIdempotencyService
{
    Task<IdempotencyBeginResult> BeginRequestAsync(
        string idempotencyKey,
        string requestPath,
        string requestHash,
        CancellationToken cancellationToken);

    Task CompleteRequestAsync(
        Guid id,
        int responseStatusCode,
        string responseBody,
        CancellationToken cancellationToken);

    Task ReleaseRequestAsync(
        Guid id,
        CancellationToken cancellationToken);
}
