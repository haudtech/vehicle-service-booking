using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Application.DTOs;

namespace VehicleServiceBooking.Application.Interfaces.Services;

/// <summary>
/// Service for managing appointment operations
/// </summary>
public interface IAppointmentService
{
    /// <summary>
    /// Creates a new appointment booking
    /// </summary>
    /// <param name="request">Appointment creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created appointment response</returns>
    /// <exception cref="InvalidOperationException">If slot is no longer available or validation fails</exception>
    Task<CreateAppointmentResponse> CreateAppointmentAsync(
        CreateAppointmentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an appointment by ID
    /// </summary>
    /// <param name="appointmentId">Appointment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Appointment details or null if not found</returns>
    Task<CreateAppointmentResponse?> GetAppointmentByIdAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels an appointment if it exists.
    /// </summary>
    /// <param name="appointmentId">Appointment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A response describing the update result</returns>
    Task<AppointmentStatusUpdateResponse?> CancelAppointmentAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes an appointment if it exists.
    /// </summary>
    /// <param name="appointmentId">Appointment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A response describing the update result</returns>
    Task<AppointmentStatusUpdateResponse?> CompleteAppointmentAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default);
}
