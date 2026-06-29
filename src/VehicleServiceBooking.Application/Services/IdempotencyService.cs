using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VehicleServiceBooking.Application.Configuration;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Services;

public class IdempotencyService : IIdempotencyService
{
    private readonly IIdempotencyRepository _idempotencyRepository;
    private readonly IdempotencyOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdempotencyService"/> class.
    /// </summary>
    public IdempotencyService(
        IIdempotencyRepository idempotencyRepository,
        IOptions<IdempotencyOptions> options)
    {
        _idempotencyRepository = idempotencyRepository ?? throw new ArgumentNullException(nameof(idempotencyRepository));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Starts idempotent request processing by creating or resolving a request record for the key and path.
    /// </summary>
    public async Task<IdempotencyBeginResult> BeginRequestAsync(
        string idempotencyKey,
        string requestPath,
        string requestHash,
        CancellationToken cancellationToken)
    {
        // Normalize key inputs so uniqueness checks are stable across callers.
        var normalizedKey = idempotencyKey.Trim();
        var normalizedPath = requestPath.Trim();

        // Resolve lookup IDs from the static status table instead of hard-coded GUIDs.
        var inProgressStatusId = await _idempotencyRepository
            .GetStatusIdByStatusAsync(IdempotencyRequestStatus.InProgress, cancellationToken)
            .ConfigureAwait(false);
        var completedStatusId = await _idempotencyRepository
            .GetStatusIdByStatusAsync(IdempotencyRequestStatus.Completed, cancellationToken)
            .ConfigureAwait(false);

        var existing = await _idempotencyRepository
            .GetByKeyAndPathAsync(normalizedKey, normalizedPath, cancellationToken)
            .ConfigureAwait(false);

        if (existing == null)
        {
            var created = new IdempotencyRequest
            {
                IdempotencyKey = normalizedKey,
                RequestPath = normalizedPath,
                RequestHash = requestHash,
                StatusId = inProgressStatusId,
                ExpiresAt = DateTime.UtcNow.AddHours(_options.EntryTtlHours)
            };

            // Try to claim this key+path first; a unique-key conflict means another request won the race.
            if (await _idempotencyRepository.TryAddAsync(created, cancellationToken).ConfigureAwait(false))
            {
                return new IdempotencyBeginResult
                {
                    Outcome = IdempotencyBeginOutcome.Proceed,
                    RecordId = created.Id
                };
            }

            existing = await _idempotencyRepository
                .GetByKeyAndPathAsync(normalizedKey, normalizedPath, cancellationToken)
                .ConfigureAwait(false);
        }

        // Extremely rare fallback: treat as in-progress when a concurrent writer exists but row is not yet visible.
        if (existing == null)
        {
            return new IdempotencyBeginResult
            {
                Outcome = IdempotencyBeginOutcome.RequestInProgress
            };
        }

        // Same key must map to the same payload; otherwise return a deterministic conflict outcome.
        if (!string.Equals(existing.RequestHash, requestHash, StringComparison.Ordinal))
        {
            return new IdempotencyBeginResult
            {
                Outcome = IdempotencyBeginOutcome.KeyReusedWithDifferentPayload
            };
        }

        // Completed requests can be replayed if response data was persisted.
        if (existing.StatusId == completedStatusId &&
            existing.ResponseStatusCode.HasValue &&
            !string.IsNullOrWhiteSpace(existing.ResponseBody))
        {
            return new IdempotencyBeginResult
            {
                Outcome = IdempotencyBeginOutcome.ReplayStoredResponse,
                RecordId = existing.Id,
                ResponseStatusCode = existing.ResponseStatusCode,
                ResponseBody = existing.ResponseBody
            };
        }

        // Any other matching row means the original request is still being processed.
        return new IdempotencyBeginResult
        {
            Outcome = IdempotencyBeginOutcome.RequestInProgress,
            RecordId = existing.Id
        };
    }

    /// <summary>
    /// Marks the idempotency request as completed and stores response data for replay.
    /// </summary>
    public Task CompleteRequestAsync(
        Guid id,
        int responseStatusCode,
        string responseBody,
        CancellationToken cancellationToken)
    {
        return _idempotencyRepository.CompleteAsync(id, responseStatusCode, responseBody, cancellationToken);
    }

    /// <summary>
    /// Releases the idempotency request record when processing should be rolled back or abandoned.
    /// </summary>
    public Task ReleaseRequestAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return _idempotencyRepository.DeleteAsync(id, cancellationToken);
    }
}
