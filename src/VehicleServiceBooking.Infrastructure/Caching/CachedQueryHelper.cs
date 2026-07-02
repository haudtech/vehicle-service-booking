using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using VehicleServiceBooking.Application.Configuration;

namespace VehicleServiceBooking.Infrastructure.Caching;

/// <summary>
/// Shared helper for cache-backed repository reads.
/// </summary>
public static class CachedQueryHelper
{
    public static async Task<IReadOnlyList<TEntity>> GetAllCachedAsync<TEntity>(
        IMemoryCache memoryCache,
        StaticDataCacheOptions options,
        ILogger logger,
        string cacheKey,
        bool isCachingEnabled,
        int ttlMinutes,
        Func<CancellationToken, Task<IEnumerable<TEntity>>> loadFromDatabaseAsync,
        string cacheFailureMessage,
        CancellationToken cancellationToken)
    {
        if (!options.Enabled || !isCachingEnabled)
        {
            var fallback = await loadFromDatabaseAsync(cancellationToken);
            return fallback as IReadOnlyList<TEntity> ?? fallback.ToList();
        }

        try
        {
            return await memoryCache.GetOrCreateAsync(
                       cacheKey,
                       async entry =>
                       {
                           entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ttlMinutes);
                           var fromInner = await loadFromDatabaseAsync(cancellationToken);
                           return fromInner as IReadOnlyList<TEntity> ?? fromInner.ToList();
                       })
                   ?? Array.Empty<TEntity>();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, cacheFailureMessage);
            var fallback = await loadFromDatabaseAsync(cancellationToken);
            return fallback as IReadOnlyList<TEntity> ?? fallback.ToList();
        }
    }
}
