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
/// Cached repository for appointment status lookups.
/// Inherits common repository methods from GenericRepository via AppointmentStatusLookupRepository.
/// </summary>
public class CachedAppointmentStatusLookupRepository : AppointmentStatusLookupRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly StaticDataCacheOptions _options;
    private readonly ILogger<CachedAppointmentStatusLookupRepository> _logger;

    public CachedAppointmentStatusLookupRepository(
        IApplicationDbContext dbContext,
        IMemoryCache memoryCache,
        IOptions<StaticDataCacheOptions> options,
        ILogger<CachedAppointmentStatusLookupRepository> logger)
        : base(dbContext)
    {
        _memoryCache = memoryCache;
        _options = options.Value;
        _logger = logger;
    }

    private async Task<IReadOnlyList<AppointmentStatusLookup>> GetAllCachedAsync(CancellationToken cancellationToken)
    {
        return await CachedQueryHelper.GetAllCachedAsync(
            _memoryCache,
            _options,
            _logger,
            StaticCacheKeys.AppointmentStatusesAll,
            _options.CacheAppointmentStatuses,
            _options.AppointmentStatusesTtlMinutes,
            ct => base.GetAllAsync(cancellationToken: ct),
            "AppointmentStatus cache read failed; falling back to database.",
            cancellationToken);
    }

    public override async Task<IEnumerable<AppointmentStatusLookup>> GetAllAsync(
        Expression<Func<AppointmentStatusLookup, bool>>? predicate = null,
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

    public override async Task<AppointmentStatusLookup?> GetByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken)
    {
        var all = await GetAllCachedAsync(cancellationToken);
        return all.FirstOrDefault(x => x.Status == status);
    }

    public override async Task<Guid?> GetStatusIdByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken)
    {
        var all = await GetAllCachedAsync(cancellationToken);
        return all.FirstOrDefault(x => x.Status == status)?.Id;
    }
}
