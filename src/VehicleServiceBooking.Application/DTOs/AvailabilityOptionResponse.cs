using System;

namespace VehicleServiceBooking.Application.DTOs;

/// <summary>
/// Single available appointment slot option
/// </summary>
/// <remarks>
/// Represents one available time slot for a requested service at a specific dealership.
/// 
/// Each AvailabilityOptionResponse includes:
/// - The exact time window (SlotStart and SlotEnd)
/// - The assigned technician who is qualified for the service
/// - The assigned service bay where the service will be performed
/// - A rank score indicating suitability (higher = better match)
/// 
/// The client should use these IDs directly in CreateAppointmentRequest to book the slot.
/// 
/// Rank Score Guide:
/// - 90-100: Highly recommended (excellent match of technician skills and bay)
/// - 70-89: Good match (technician is qualified, bay is suitable)
/// - 50-69: Acceptable match (meets requirements but suboptimal)
/// - Below 50: Fallback option (limited availability)
/// 
/// Note: Rank scores are for client-side UI sorting/recommendations only.
/// Any available slot is equally valid from the system perspective.
/// </remarks>
public class AvailabilityOptionResponse
{
    /// <summary>
    /// Appointment start time (UTC)
    /// </summary>
    /// <remarks>
    /// The exact moment when the service appointment would begin.
    /// Format: ISO 8601 (e.g., "2026-06-25T10:00:00Z")
    /// </remarks>
    public DateTime SlotStart { get; set; }

    /// <summary>
    /// Appointment end time (UTC)
    /// </summary>
    /// <remarks>
    /// The exact moment when the service appointment would end.
    /// Duration = SlotEnd - SlotStart
    /// Format: ISO 8601 (e.g., "2026-06-25T11:00:00Z")
    /// </remarks>
    public DateTime SlotEnd { get; set; }

    /// <summary>
    /// Technician who would perform the service
    /// </summary>
    /// <remarks>
    /// ID of the technician assigned to this slot.
    /// This technician is guaranteed to:
    /// - Be available during this time window
    /// - Have the required skills for the requested service type
    /// - Not have conflicting appointments
    /// 
    /// Use this ID in CreateAppointmentRequest.TechnicianId to book this slot.
    /// </remarks>
    public Guid TechnicianId { get; set; }

    /// <summary>
    /// Service bay where service would happen
    /// </summary>
    /// <remarks>
    /// ID of the service bay assigned to this slot.
    /// This service bay is guaranteed to:
    /// - Be available during this time window
    /// - Have no conflicting appointments
    /// - Be suitable for the requested service type
    /// 
    /// Use this ID in CreateAppointmentRequest.ServiceBayId to book this slot.
    /// </remarks>
    public Guid ServiceBayId { get; set; }

    /// <summary>
    /// Rank score for sorting and recommendations (0-100)
    /// </summary>
    /// <remarks>
    /// Quality score indicating how well this option matches the request.
    /// 
    /// Scoring factors:
    /// - Technician skill level for the service type
    /// - Service bay suitability and equipment
    /// - Technician availability patterns
    /// - Bay utilization efficiency
    /// 
    /// Use this for:
    /// - Sorting options in UI (highest rank first)
    /// - Highlighting recommendations ("Best Available" badge)
    /// - Client-side recommendations only
    /// 
    /// Note: All options are equally valid; rank is for UX purposes only.
    /// Value range: 0-100 where 100 is perfect match
    /// </remarks>
    public int RankScore { get; set; }
}
