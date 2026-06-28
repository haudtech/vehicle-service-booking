using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for ServiceBay entity persistence operations
/// </summary>
public interface IServiceBayRepository : IReadRepository<ServiceBay>, IWriteRepository<ServiceBay>
{
    /// <summary>
    /// Gets all service bays for a specific dealership
    /// </summary>
    /// <param name="dealershipId">The dealership ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of service bays for the dealership</returns>
    Task<IEnumerable<ServiceBay>> GetByDealershipAsync(
        Guid dealershipId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets all available service bays for a dealership (no appointments in time range)
    /// </summary>
    /// <param name="dealershipId">The dealership ID</param>
    /// <param name="startTime">Start time of the range</param>
    /// <param name="endTime">End time of the range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of available service bays</returns>
    Task<IEnumerable<ServiceBay>> GetAvailableAsync(
        Guid dealershipId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken);
}
