using System;

namespace VehicleServiceBooking.Application.DTOs;

/// <summary>
/// Standard error response format
/// </summary>
/// <remarks>
/// All error responses from the API use this standard format for consistency.
/// 
/// Error Code Reference:
/// 
/// **Validation Errors (400 Bad Request):**
/// - INVALID_APPOINTMENT_ID: Appointment ID is empty/invalid
/// - INVALID_REQUEST: Request validation failed
/// - INVALID_DEALERSHIP_ID: DealershipId is empty/invalid
/// - INVALID_CUSTOMER_ID: CustomerId is empty/invalid
/// - INVALID_VEHICLE_ID: VehicleId is empty/invalid
/// - INVALID_SERVICE_TYPE_ID: ServiceTypeId is empty/invalid
/// - INVALID_TECHNICIAN_ID: TechnicianId is empty/invalid
/// - INVALID_SERVICE_BAY_ID: ServiceBayId is empty/invalid
/// - INVALID_TIME_SLOT: SlotStart/SlotEnd validation failed
/// 
/// **Conflict Errors (409 Conflict):**
/// - SLOT_NO_LONGER_AVAILABLE: Selected slot already booked
/// - SLOT_CONFLICT: Time slot conflict detected
/// 
/// **Not Found Errors (404 Not Found):**
/// - APPOINTMENT_NOT_FOUND: Requested appointment doesn't exist
/// 
/// **Server Errors (500 Internal Server Error):**
/// - INTERNAL_ERROR: Unexpected server error
/// 
/// Client applications should use the ErrorCode field for:
/// - Logging and error tracking
/// - Conditional error handling
/// - User experience customization (showing appropriate messages/actions per error type)
/// </remarks>
public class ErrorResponse
{
    /// <summary>
    /// User-friendly error message
    /// </summary>
    /// <remarks>
    /// Human-readable message suitable for display to end users.
    /// Should describe what went wrong and possibly suggest a resolution.
    /// Example: "The selected time slot is no longer available. Please select another slot."
    /// </remarks>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Machine-readable error code for programmatic handling
    /// </summary>
    /// <remarks>
    /// Standardized error codes that applications can use for error handling logic.
    /// Format: UPPERCASE_SNAKE_CASE
    /// Example: SLOT_NO_LONGER_AVAILABLE
    /// 
    /// Use this field in client code for:
    /// - Conditional error handling
    /// - Logging categorization
    /// - User experience customization
    /// - Retry logic decisions
    /// </remarks>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Server timestamp when the error occurred
    /// </summary>
    /// <remarks>
    /// ISO 8601 formatted UTC timestamp.
    /// Useful for:
    /// - Correlating with server logs
    /// - Understanding error sequence in multi-call flows
    /// - User support (helping support team locate events in logs)
    /// Format: ISO 8601 (e.g., "2026-06-25T09:30:00Z")
    /// </remarks>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
