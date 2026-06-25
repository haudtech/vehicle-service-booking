using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for ServiceBay entity persistence operations
/// </summary>
public interface IServiceBayRepository
{
    /// <summary>
    /// Gets a service bay by its unique identifier
    /// </summary>
    /// <param name="id">The service bay ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The service bay or null if not found</returns>
    Task<ServiceBay?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

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

    /// <summary>
    /// Adds a new service bay to the repository
    /// </summary>
    /// <param name="serviceBay">The service bay to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added service bay with generated ID</returns>
    Task<ServiceBay> AddAsync(ServiceBay serviceBay, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing service bay
    /// </summary>
    /// <param name="serviceBay">The service bay with updated values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated service bay</returns>
    Task<ServiceBay> UpdateAsync(ServiceBay serviceBay, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a service bay by ID
    /// </summary>
    /// <param name="id">The service bay ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Saves all pending changes to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
