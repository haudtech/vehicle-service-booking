using System;

namespace VehicleServiceBooking.Application.DTOs;

/// <summary>
/// Request to query available appointment slots
/// </summary>
/// <remarks>
/// Use this DTO to search for available appointment slots at a specific dealership
/// for a specific service type on a specific date.
/// 
/// The availability search is:
/// - **Dealership-specific**: Only shows slots at the requested dealership
/// - **Service-specific**: Only shows technicians qualified for the service type
/// - **Date-specific**: Only shows slots within the business hours of that date
/// 
/// Typical workflow:
/// 1. Client queries availability using this request
/// 2. System returns list of AvailabilityOptionResponse objects
/// 3. Client selects one option
/// 4. Client uses that option's technicianId and serviceBayId in CreateAppointmentRequest
/// 5. Appointment is created with guaranteed slot availability
/// </remarks>
public class GetAvailabilityRequest
{
    /// <summary>
    /// Dealership ID (which dealership to query)
    /// </summary>
    /// <remarks>
    /// Required. Must be a valid GUID of an existing dealership.
    /// The availability search will only return slots from this dealership.
    /// </remarks>
    public Guid DealershipId { get; set; }

    /// <summary>
    /// Service type ID (what type of service needed)
    /// </summary>
    /// <remarks>
    /// Required. Must be a valid GUID of an existing service type.
    /// The system will only include technicians who are qualified for this service.
    /// The service type also determines the standard duration of the appointment.
    /// </remarks>
    public Guid ServiceTypeId { get; set; }

    /// <summary>
    /// Date to find available slots for
    /// </summary>
    /// <remarks>
    /// Required. The date (ignoring time) for which to search for available slots.
    /// The system will find all available slots within the dealership's business hours for this date.
    /// Format: ISO 8601 date (e.g., "2026-06-25") or datetime (time component will be ignored)
    /// </remarks>
    public DateTime Date { get; set; }
}
