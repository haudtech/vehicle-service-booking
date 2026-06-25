using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for availability checking and slot management
/// </summary>
public interface IAvailabilityRepository
{
    /// <summary>
    /// Gets all appointments for a service type within a time range
    /// </summary>
    /// <param name="serviceTypeId">The service type ID</param>
    /// <param name="startTime">Start time of the range</param>
    /// <param name="endTime">End time of the range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of appointments for the service type</returns>
    Task<IEnumerable<Appointment>> GetAppointmentsByServiceTypeAsync(
        Guid serviceTypeId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets all technicians for a dealership with their schedules
    /// </summary>
    /// <param name="dealershipId">The dealership ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of technicians with their schedules</returns>
    Task<IEnumerable<Technician>> GetTechniciansByDealershipAsync(
        Guid dealershipId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets all technicians with specific skills
    /// </summary>
    /// <param name="serviceTypeId">The service type ID requiring specific skills</param>
    /// <param name="dealershipId">The dealership ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of qualified technicians</returns>
    Task<IEnumerable<Technician>> GetQualifiedTechniciansAsync(
        Guid serviceTypeId,
        Guid dealershipId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets all service bays for availability checking
    /// </summary>
    /// <param name="dealershipId">The dealership ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of service bays</returns>
    Task<IEnumerable<ServiceBay>> GetServiceBaysAsync(
        Guid dealershipId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets all appointments within a time range
    /// </summary>
    /// <param name="startTime">Start time of the range</param>
    /// <param name="endTime">End time of the range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of appointments within the range</returns>
    Task<IEnumerable<Appointment>> GetAppointmentsInRangeAsync(
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets business hours for a dealership
    /// </summary>
    /// <param name="dealershipId">The dealership ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of business hours</returns>
    Task<IEnumerable<BusinessHours>> GetBusinessHoursAsync(
        Guid dealershipId,
        CancellationToken cancellationToken);
}
