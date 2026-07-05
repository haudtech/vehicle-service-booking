namespace VehicleServiceBooking.Auth.Repositories.Interfaces;

/// <summary>
/// Defines common write operations for repository entities.
/// </summary>
public interface IWriteRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Adds an entity to the current unit of work.
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an entity in the current unit of work.
    /// </summary>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists pending changes to storage.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
