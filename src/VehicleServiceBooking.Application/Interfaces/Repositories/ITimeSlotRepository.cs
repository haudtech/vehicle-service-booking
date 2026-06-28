using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for TimeSlot entity persistence and lookup operations.
/// </summary>
public interface ITimeSlotRepository : IReadRepository<TimeSlot>
{
    /// <summary>
    /// Gets time slots in a given sequence range.
    /// </summary>
    /// <param name="startSequence">Starting sequence order.</param>
    /// <param name="endSequence">Ending sequence order.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The matching time slot entities.</returns>
    Task<IReadOnlyList<TimeSlot>> GetBySequenceRangeAsync(
        int startSequence,
        int endSequence,
        CancellationToken cancellationToken);
}
