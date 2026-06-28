using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Abstract base repository providing common CRUD and query operations
/// with consistent AsNoTracking() configuration for all derived repositories
/// </summary>
/// <typeparam name="TEntity">The entity type managed by this repository</typeparam>
public abstract class GenericRepository<TEntity> : IReadRepository<TEntity>, IWriteRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Gets the database context
    /// </summary>
    protected readonly IApplicationDbContext DbContext;

    /// <summary>
    /// Initializes a new instance of the GenericRepository class
    /// </summary>
    /// <param name="dbContext">The application database context</param>
    /// <exception cref="ArgumentNullException">Thrown when dbContext is null</exception>
    protected GenericRepository(IApplicationDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Gets a queryable collection with AsNoTracking() applied by default.
    /// Use this as the base for all read queries to ensure consistent configuration.
    /// </summary>
    /// <returns>An IQueryable collection with change tracking disabled</returns>
    protected IQueryable<TEntity> GetQueryable()
    {
        return DbContext.DbContext.Set<TEntity>().AsNoTracking();
    }

    /// <summary>
    /// Executes a query with the standard NoTracking configuration
    /// Useful when you need to apply complex query logic while ensuring consistency
    /// </summary>
    /// <typeparam name="TResult">The result type of the query</typeparam>
    /// <param name="queryFunc">The query function to execute</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>The result of the query execution</returns>
    protected async Task<TResult> ExecuteQueryAsync<TResult>(
        Func<IQueryable<TEntity>, Task<TResult>> queryFunc,
        CancellationToken cancellationToken)
    {
        return await queryFunc(GetQueryable());
    }

    /// <summary>
    /// Gets an entity by its primary key (Id)
    /// Override this method in derived classes if entity doesn't have an Id property or if you need includes
    /// </summary>
    /// <param name="id">The entity's primary key</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>The entity if found; otherwise null</returns>
    public virtual async Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await GetQueryable()
            .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
    }

    /// <summary>
    /// Gets entities by their primary keys (Id).
    /// </summary>
    /// <param name="ids">The entity primary keys.</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>A collection of matching entities</returns>
    public virtual async Task<IEnumerable<TEntity>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
        {
            return Array.Empty<TEntity>();
        }

        return await GetQueryable()
            .Where(e => idList.Contains(EF.Property<Guid>(e, "Id")))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the first entity matching the specified predicate
    /// </summary>
    /// <param name="predicate">The filter condition</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>The first matching entity or null if no match found</returns>
    public virtual async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken)
    {
        return await GetQueryable()
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Checks if any entity matches the specified predicate
    /// </summary>
    /// <param name="predicate">The filter condition</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>True if any entity matches; otherwise false</returns>
    public virtual async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken)
    {
        return await GetQueryable()
            .AnyAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Gets all entities matching the specified predicate
    /// </summary>
    /// <param name="predicate">The filter condition</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>A collection of matching entities</returns>
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryable();
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a new entity to the database
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>The added entity</returns>
    public virtual async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken)
    {
        DbContext.DbContext.Set<TEntity>().Add(entity);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <summary>
    /// Adds multiple entities to the database in a single operation
    /// </summary>
    /// <param name="entities">The entities to add</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>The added entities</returns>
    public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken)
    {
        var entityList = entities.ToList();
        DbContext.DbContext.Set<TEntity>().AddRange(entityList);
        await SaveChangesAsync(cancellationToken);
        return entityList;
    }

    /// <summary>
    /// Updates an existing entity in the database
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>The updated entity</returns>
    public virtual async Task<TEntity> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken)
    {
        DbContext.DbContext.Set<TEntity>().Update(entity);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <summary>
    /// Deletes an entity from the database by its primary key
    /// </summary>
    /// <param name="id">The entity's primary key</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    public virtual async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            DbContext.DbContext.Set<TEntity>().Remove(entity);
            await SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Deletes an entity from the database
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    public virtual async Task DeleteAsync(
        TEntity entity,
        CancellationToken cancellationToken)
    {
        DbContext.DbContext.Set<TEntity>().Remove(entity);
        await SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Saves all pending changes to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await DbContext.SaveChangesAsync(cancellationToken);
    }
}
