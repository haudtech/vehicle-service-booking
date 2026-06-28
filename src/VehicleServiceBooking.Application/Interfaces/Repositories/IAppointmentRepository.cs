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
    /// Gets all appointments for a specific vehicle
    /// Used for conflict detection and vehicle history queries
    /// </summary>
    /// <param name="vehicleId">The vehicle ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of appointments for the vehicle with services included</returns>
    Task<IEnumerable<Appointment>> GetByVehicleIdAsync(
        Guid vehicleId,
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

    // ==================== PHASE 4: MULTI-SERVICE SUPPORT METHODS ====================

    /// <summary>
    /// Check if a service bay is available for a specific time slot on a given date
    /// Uses ServiceBayAvailableSlots view to verify no conflicts
    /// 
    /// Business Logic: NONE - Pure data query from view
    /// Performance: Single view query (< 10ms)
    /// Used by: AppointmentService to validate bay assignment
    /// </summary>
    /// <param name="serviceBayId">The service bay ID to check</param>
    /// <param name="startSequenceOrder">Starting TimeSlot sequence order</param>
    /// <param name="endSequenceOrder">Ending TimeSlot sequence order</param>
    /// <param name="appointmentDate">The date of appointment</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if bay is available for entire duration, false otherwise</returns>
    Task<bool> IsBayAvailableForSlotAsync(
        Guid serviceBayId,
        int startSequenceOrder,
        int endSequenceOrder,
        DateOnly appointmentDate,
        CancellationToken cancellationToken);

    /// <summary>
    /// Verify that a technician has the required skill for a specific service type
    /// Uses TechnicianSkill entity to validate qualification
    /// 
    /// Business Logic: NONE - Pure data query
    /// Used by: AppointmentService to validate technician assignment
    /// </summary>
    /// <param name="technicianId">The technician ID</param>
    /// <param name="serviceTypeId">The service type ID requiring the skill</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if technician has skill, false otherwise</returns>
    Task<bool> TechnicianHasSkillAsync(
        Guid technicianId,
        Guid serviceTypeId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Create an appointment atomically with all its services in a single transaction
    /// Handles: Adding Appointment + Services + TimeSlotAssignments in one go
    /// 
    /// Business Logic: NONE - Pure persistence
    /// Transaction: ACID - All or nothing
    /// Used by: AppointmentService when creating multi-service appointments
    /// </summary>
    /// <param name="appointment">The appointment to create (with Services collection)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created appointment with generated ID and all services</returns>
    Task<Appointment> CreateAppointmentWithServicesAsync(
        Appointment appointment,
        CancellationToken cancellationToken);

    /// <summary>
    /// Get all TimeSlot IDs for a duration-based range
    /// Used to populate appointment's TimeSlotAssignments
    /// 
    /// Business Logic: NONE - Pure data query
    /// Used by: AppointmentService when creating appointment service records
    /// </summary>
    /// <param name="startSequenceOrder">Starting sequence order</param>
    /// <param name="endSequenceOrder">Ending sequence order (inclusive)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of TimeSlot entities in sequence order</returns>
    Task<IEnumerable<TimeSlot>> GetTimeSlotsBySequenceRangeAsync(
        int startSequenceOrder,
        int endSequenceOrder,
        CancellationToken cancellationToken);
}
