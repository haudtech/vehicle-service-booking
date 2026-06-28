using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for TimeSlot entity.
/// </summary>
public class TimeSlotRepository : GenericRepository<TimeSlot>, ITimeSlotRepository
{
    public TimeSlotRepository(IApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public virtual async Task<IReadOnlyList<TimeSlot>> GetBySequenceRangeAsync(
        int startSequence,
        int endSequence,
        CancellationToken cancellationToken)
    {
        return await GetQueryable()
            .Where(ts => ts.SequenceOrder >= startSequence && ts.SequenceOrder <= endSequence)
            .OrderBy(ts => ts.SequenceOrder)
            .ToListAsync(cancellationToken);
    }
}
