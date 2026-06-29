using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

public interface IIdempotencyRepository
{
    Task<Guid> GetStatusIdByStatusAsync(
        IdempotencyRequestStatus status,
        CancellationToken cancellationToken);

    Task<IdempotencyRequest?> GetByKeyAndPathAsync(
        string idempotencyKey,
        string requestPath,
        CancellationToken cancellationToken);

    Task<bool> TryAddAsync(
        IdempotencyRequest request,
        CancellationToken cancellationToken);

    Task CompleteAsync(
        Guid id,
        int responseStatusCode,
        string responseBody,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);
}
