using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Application.Models;
using VehicleServiceBooking.Application.Models.ViewModels;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for availability checking and slot management
/// </summary>
public interface IAvailabilityRepository
{
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
    /// <returns>Collection of projected availability rows with only required fields</returns>
    Task<IEnumerable<AvailabilityProjection>> GetServiceTypeAvailabilityAsync(
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
    Task<IEnumerable<TechnicianAvailableSlotsView>> GetTechnicianAvailableSlotsAsync(
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
    Task<IEnumerable<ServiceBayAvailableSlotsView>> GetServiceBayAvailableSlotsAsync(
        Guid dealershipId,
        DateOnly queryDate,
        CancellationToken cancellationToken);
}
