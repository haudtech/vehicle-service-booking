using System.Linq.Expressions;

namespace VehicleServiceBooking.Auth.Repositories.Interfaces;

/// <summary>
/// Defines common read operations for repository entities.
/// </summary>
public interface IReadRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Gets an entity by identifier.
    /// </summary>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the first entity matching a predicate.
    /// </summary>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities optionally filtered by predicate.
    /// </summary>
    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
}
