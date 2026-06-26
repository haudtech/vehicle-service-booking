using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Application.Models.ViewModels;
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

    // ==================== PHASE 4: MATERIALIZED VIEW QUERY METHODS ====================

    /// <summary>
    /// Query ServiceTypeAvailability view for all service types and their available options
    /// Returns pre-computed availability with service duration, technician, and bay combinations
    /// 
    /// Business Logic: NONE - Pure data query from materialized view
    /// Performance: Single database query (< 50ms) vs. 5+ queries in old approach
    /// </summary>
    /// <param name="dealershipId">The dealership ID</param>
    /// <param name="serviceTypeIds">Array of service type IDs to query (supports multi-service)</param>
    /// <param name="queryDate">The date to query availability for (business logic filters to this date)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of ServiceTypeAvailabilityView records with availability options</returns>
    Task<IEnumerable<ServiceTypeAvailabilityView>> GetServiceTypeAvailabilityAsync(
        Guid dealershipId,
        Guid[] serviceTypeIds,
        DateOnly queryDate,
        CancellationToken cancellationToken);

    /// <summary>
    /// Query TechnicianAvailableSlots view for available slots per technician
    /// Returns pre-computed available time slots for qualified technicians
    /// 
    /// Business Logic: NONE - Pure data query from materialized view
    /// Used by: AvailabilityService to build availability options and map to DTOs
    /// </summary>
    /// <param name="dealershipId">The dealership ID</param>
    /// <param name="queryDate">The date to query availability for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of available slots per technician</returns>
    Task<IEnumerable<TechnicianAvailableSlot>> GetTechnicianAvailableSlotsAsync(
        Guid dealershipId,
        DateOnly queryDate,
        CancellationToken cancellationToken);

    /// <summary>
    /// Query ServiceBayAvailableSlots view for available slots per service bay
    /// Returns pre-computed available time slots for service bays
    /// 
    /// Business Logic: NONE - Pure data query from materialized view
    /// Used by: AvailabilityService and CreateAppointment logic for bay assignment
    /// </summary>
    /// <param name="dealershipId">The dealership ID</param>
    /// <param name="queryDate">The date to query availability for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of available slots per service bay</returns>
    Task<IEnumerable<ServiceBayAvailableSlot>> GetServiceBayAvailableSlotsAsync(
        Guid dealershipId,
        DateOnly queryDate,
        CancellationToken cancellationToken);

    /// <summary>
    /// Find an available service bay for a given time slot range
    /// Uses ServiceBayAvailableSlots view to find first available bay that can fit the duration
    /// 
    /// Business Logic: NONE - Pure data query returning first match
    /// Returns: ServiceBay entity for the matched bay (loaded separately if needed)
    /// </summary>
    /// <param name="dealershipId">The dealership ID</param>
    /// <param name="startSlotSequence">Starting TimeSlot sequence order</param>
    /// <param name="endSlotSequence">Ending TimeSlot sequence order</param>
    /// <param name="appointmentDate">The appointment date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ServiceBay entity if available, null otherwise</returns>
    Task<ServiceBay?> FindAvailableBayAsync(
        Guid dealershipId,
        int startSlotSequence,
        int endSlotSequence,
        DateOnly appointmentDate,
        CancellationToken cancellationToken);

    /// <summary>
    /// Get TimeSlot by sequence order and resolve actual start/end times
    /// Used to convert sequence order to TimeSlot entities for appointments
    /// 
    /// Business Logic: NONE - Pure data query
    /// Performance: Single query to load TimeSlot entity
    /// </summary>
    /// <param name="sequenceOrder">The TimeSlot sequence order (1-18)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>TimeSlot entity if found, null otherwise</returns>
    Task<TimeSlot?> GetTimeSlotBySequenceAsync(
        int sequenceOrder,
        CancellationToken cancellationToken);

    /// <summary>
    /// Get a range of TimeSlots by sequence order
    /// Used to determine all TimeSlots needed for an appointment of given duration
    /// 
    /// Business Logic: NONE - Pure data query
    /// Performance: Single query loading multiple TimeSlots
    /// </summary>
    /// <param name="startSequence">Starting TimeSlot sequence order</param>
    /// <param name="endSequence">Ending TimeSlot sequence order (inclusive)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of TimeSlot entities in sequence order</returns>
    Task<IEnumerable<TimeSlot>> GetTimeSlotRangeAsync(
        int startSequence,
        int endSequence,
        CancellationToken cancellationToken);
}
