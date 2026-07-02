using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VehicleServiceBooking.Application.Configuration;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;
using VehicleServiceBooking.Infrastructure.Caching;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Cached repository for service status lookups.
/// Inherits common repository methods from GenericRepository via ServiceStatusLookupRepository.
/// </summary>
public class CachedServiceStatusLookupRepository : ServiceStatusLookupRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly StaticDataCacheOptions _options;
    private readonly ILogger<CachedServiceStatusLookupRepository> _logger;

    public CachedServiceStatusLookupRepository(
        IApplicationDbContext dbContext,
        IMemoryCache memoryCache,
        IOptions<StaticDataCacheOptions> options,
        ILogger<CachedServiceStatusLookupRepository> logger)
        : base(dbContext)
    {
        _memoryCache = memoryCache;
        _options = options.Value;
        _logger = logger;
    }

    private async Task<IReadOnlyList<ServiceStatusLookup>> GetAllCachedAsync(CancellationToken cancellationToken)
    {
        return await CachedQueryHelper.GetAllCachedAsync(
            _memoryCache,
            _options,
            _logger,
            StaticCacheKeys.ServiceStatusesAll,
            _options.CacheServiceStatuses,
            _options.ServiceStatusesTtlMinutes,
            ct => base.GetAllAsync(cancellationToken: ct),
            "ServiceStatus cache read failed; falling back to database.",
            cancellationToken);
    }

    public override async Task<IEnumerable<ServiceStatusLookup>> GetAllAsync(
        Expression<Func<ServiceStatusLookup, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var all = await GetAllCachedAsync(cancellationToken);
        if (predicate == null)
        {
            return all;
        }

        var compiled = predicate.Compile();
        return all.Where(compiled).ToList();
    }

    public override async Task<ServiceStatusLookup?> GetByStatusAsync(ServiceStatus status, CancellationToken cancellationToken)
    {
        var all = await GetAllCachedAsync(cancellationToken);
        return all.FirstOrDefault(x => x.Status == status);
    }
}
