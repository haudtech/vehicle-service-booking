using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Write-side repository contract for common entity persistence operations.
/// </summary>
/// <typeparam name="TEntity">The entity type managed by the repository.</typeparam>
public interface IWriteRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);

    Task<IEnumerable<TEntity>> AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken);

    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken);

    Task DeleteAsync(System.Guid id, CancellationToken cancellationToken);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}