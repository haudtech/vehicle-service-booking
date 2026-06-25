using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Appointment entity persistence operations
/// </summary>
public interface IAppointmentRepository
{
    /// <summary>
    /// Gets an appointment by its unique identifier
    /// </summary>
    /// <param name="id">The appointment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The appointment or null if not found</returns>
    Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Gets all appointments for a specific service bay within a time range
    /// </summary>
    /// <param name="serviceBayId">The service bay ID</param>
    /// <param name="startTime">Start time of the range</param>
    /// <param name="endTime">End time of the range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of appointments within the specified range</returns>
    Task<IEnumerable<Appointment>> GetByServiceBayAsync(
        Guid serviceBayId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets all appointments for a specific dealership
    /// </summary>
    /// <param name="dealershipId">The dealership ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of appointments for the dealership</returns>
    Task<IEnumerable<Appointment>> GetByDealershipAsync(
        Guid dealershipId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new appointment to the repository
    /// </summary>
    /// <param name="appointment">The appointment to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added appointment with generated ID</returns>
    Task<Appointment> AddAsync(Appointment appointment, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing appointment
    /// </summary>
    /// <param name="appointment">The appointment with updated values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated appointment</returns>
    Task<Appointment> UpdateAsync(Appointment appointment, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an appointment by ID
    /// </summary>
    /// <param name="id">The appointment ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Saves all pending changes to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
