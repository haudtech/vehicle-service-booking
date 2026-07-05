using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Auth.Data;
using VehicleServiceBooking.Auth.Repositories.Interfaces;

namespace VehicleServiceBooking.Auth.Repositories;

/// <summary>
/// Base repository with shared query and persistence operations for auth entities.
/// </summary>
public abstract class GenericRepository<TEntity> : IReadRepository<TEntity>, IWriteRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepository{TEntity}"/> class.
    /// </summary>
    protected GenericRepository(AuthDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    protected AuthDbContext DbContext { get; }

    /// <summary>
    /// Gets an untracked query for the repository entity type.
    /// </summary>
    protected IQueryable<TEntity> GetQueryable()
    {
        return DbContext.Set<TEntity>().AsNoTracking();
    }

    /// <summary>
    /// Executes custom query logic using the default no-tracking query.
    /// </summary>
    protected async Task<TResult> ExecuteQueryAsync<TResult>(
        Func<IQueryable<TEntity>, Task<TResult>> queryFunc,
        CancellationToken cancellationToken = default)
    {
        return await queryFunc(GetQueryable());
    }

    /// <inheritdoc />
    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetQueryable().FirstOrDefaultAsync(entity => EF.Property<Guid>(entity, "Id") == id, cancellationToken);
    }

    /// <summary>
    /// Gets entities by identifier set.
    /// </summary>
    public virtual async Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
        {
            return Array.Empty<TEntity>();
        }

        return await GetQueryable()
            .Where(entity => idList.Contains(EF.Property<Guid>(entity, "Id")))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await GetQueryable().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Determines whether any entity matches the specified predicate.
    /// </summary>
    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await GetQueryable().AnyAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = GetQueryable();
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <summary>
    /// Adds multiple entities to the current unit of work.
    /// </summary>
    public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();
        await DbContext.Set<TEntity>().AddRangeAsync(entityList, cancellationToken);
        return entityList;
    }

    /// <inheritdoc />
    public virtual Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<TEntity>().Update(entity);
        return Task.FromResult(entity);
    }

    /// <summary>
    /// Deletes an entity by identifier from the current unit of work.
    /// </summary>
    public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        DbContext.Set<TEntity>().Remove(entity);
    }

    /// <summary>
    /// Deletes an entity from the current unit of work.
    /// </summary>
    public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<TEntity>().Remove(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return DbContext.SaveChangesAsync(cancellationToken);
    }
}
