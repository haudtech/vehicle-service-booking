using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Infrastructure.Repositories;

public class IdempotencyRepository : IIdempotencyRepository
{
    private readonly IApplicationDbContext _dbContext;

    public IdempotencyRepository(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Guid> GetStatusIdByStatusAsync(
        IdempotencyRequestStatus status,
        CancellationToken cancellationToken)
    {
        var statusId = await _dbContext.IdempotencyRequestStatusLookups
            .AsNoTracking()
            .Where(x => x.Status == status)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (!statusId.HasValue)
        {
            throw new InvalidOperationException($"Idempotency status lookup row for '{status}' was not found.");
        }

        return statusId.Value;
    }

    public async Task<IdempotencyRequest?> GetByKeyAndPathAsync(
        string idempotencyKey,
        string requestPath,
        CancellationToken cancellationToken)
    {
        return await _dbContext.IdempotencyRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.IdempotencyKey == idempotencyKey && x.RequestPath == requestPath,
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> TryAddAsync(
        IdempotencyRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _dbContext.IdempotencyRequests.Add(request);
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            return false;
        }
    }

    public async Task CompleteAsync(
        Guid id,
        int responseStatusCode,
        string responseBody,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.IdempotencyRequests
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);

        if (entity == null)
        {
            return;
        }

        var completedStatusId = await GetStatusIdByStatusAsync(IdempotencyRequestStatus.Completed, cancellationToken)
            .ConfigureAwait(false);

        entity.StatusId = completedStatusId;
        entity.ResponseStatusCode = responseStatusCode;
        entity.ResponseBody = responseBody;
        entity.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.IdempotencyRequests
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);

        if (entity == null)
        {
            return;
        }

        _dbContext.IdempotencyRequests.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        if (ex.InnerException is PostgresException pgEx)
        {
            return pgEx.SqlState == PostgresErrorCodes.UniqueViolation;
        }

        return false;
    }
}
