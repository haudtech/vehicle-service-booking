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
using VehicleServiceBooking.Infrastructure.Caching;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Cached repository for time slot reads.
/// Inherits common repository methods from GenericRepository via TimeSlotRepository.
/// </summary>
public class CachedTimeSlotRepository : TimeSlotRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly StaticDataCacheOptions _options;
    private readonly ILogger<CachedTimeSlotRepository> _logger;

    public CachedTimeSlotRepository(
        IApplicationDbContext dbContext,
        IMemoryCache memoryCache,
        IOptions<StaticDataCacheOptions> options,
        ILogger<CachedTimeSlotRepository> logger)
        : base(dbContext)
    {
        _memoryCache = memoryCache;
        _options = options.Value;
        _logger = logger;
    }

    public override async Task<IReadOnlyList<TimeSlot>> GetBySequenceRangeAsync(
        int startSequence,
        int endSequence,
        CancellationToken cancellationToken)
    {
        var all = await GetAllCachedAsync(cancellationToken);

        return all
            .Where(x => x.SequenceOrder >= startSequence && x.SequenceOrder <= endSequence)
            .OrderBy(x => x.SequenceOrder)
            .ToList();
    }

    private async Task<IReadOnlyList<TimeSlot>> GetAllCachedAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled || !_options.CacheTimeSlots)
        {
            var fallback = await base.GetAllAsync(cancellationToken: cancellationToken);
            return fallback as IReadOnlyList<TimeSlot> ?? fallback.ToList();
        }

        try
        {
            return await _memoryCache.GetOrCreateAsync(
                       StaticCacheKeys.TimeSlotsAll,
                       async entry =>
                       {
                           entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.TimeSlotsTtlMinutes);
                           var fromInner = await base.GetAllAsync(cancellationToken: cancellationToken);
                           return fromInner as IReadOnlyList<TimeSlot> ?? fromInner.ToList();
                       })
                   ?? Array.Empty<TimeSlot>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "TimeSlot cache read failed; falling back to database.");
            var fallback = await base.GetAllAsync(cancellationToken: cancellationToken);
            return fallback as IReadOnlyList<TimeSlot> ?? fallback.ToList();
        }
    }

    public override async Task<IEnumerable<TimeSlot>> GetAllAsync(
        Expression<Func<TimeSlot, bool>>? predicate = null,
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
}
