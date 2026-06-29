using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VehicleServiceBooking.Api.Services;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Exceptions;
using VehicleServiceBooking.Application.Interfaces;
using VehicleServiceBooking.Application.Interfaces.Services;

namespace VehicleServiceBooking.Api.Controllers;

/// <summary>
/// Manages appointment creation and retrieval for vehicle service bookings
/// </summary>
/// <remarks>
/// This controller handles the complete appointment lifecycle:
/// - Creating new service appointments
/// - Retrieving appointment details by ID
/// - Validating availability and slot conflicts
/// 
/// All responses include proper HTTP status codes and error details.
/// </remarks>
[ApiController]
[Route("api/v1")]
[Produces("application/json")]
[Tags("Appointments")]
public class AppointmentsController : ControllerBase
{
    private static readonly JsonSerializerOptions ReplayJsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IValidator<CreateAppointmentRequest> _createValidator;
    private readonly IAppointmentService _appointmentService;
    private readonly IIdempotencyService _idempotencyService;
    private readonly IIdempotencyRequestCoordinator _idempotencyRequestCoordinator;
    private readonly ILogger<AppointmentsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppointmentsController"/> class.
    /// </summary>
    public AppointmentsController(
        IValidator<CreateAppointmentRequest> createValidator,
        IAppointmentService appointmentService,
        IIdempotencyService idempotencyService,
        IIdempotencyRequestCoordinator idempotencyRequestCoordinator,
        ILogger<AppointmentsController> logger)
    {
        _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
        _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
        _idempotencyService = idempotencyService ?? throw new ArgumentNullException(nameof(idempotencyService));
        _idempotencyRequestCoordinator = idempotencyRequestCoordinator ?? throw new ArgumentNullException(nameof(idempotencyRequestCoordinator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Create a new appointment booking
    /// </summary>
    /// <remarks>
    /// Creates a new vehicle service appointment with the provided details.
    /// 
    /// The endpoint performs the following validations:
    /// - Validates all required fields are present and properly formatted
    /// - Checks that the selected time slot is available for the specified service bay
    /// - Ensures no conflicting appointments exist for the same service bay
    /// - Verifies all referenced entities (customer, vehicle, service type, technician, service bay) exist
    /// - When idempotency is enabled, clients should send the `Idempotency-Key` header with a unique value
    /// 
    /// ## Request Requirements:
    /// - Idempotency-Key: Optional unless idempotency is configured to require it; send a unique value for each logical create attempt
    /// - DealershipId: Must be a valid GUID
    /// - CustomerId: Must be a valid GUID and the customer must exist
    /// - VehicleId: Must be a valid GUID and the vehicle must exist
    /// - ServiceTypeId: Must be a valid GUID and the service type must exist
    /// - TechnicianId: Must be a valid GUID and the technician must exist
    /// - ServiceBayId: Must be a valid GUID and the service bay must exist
    /// - SlotStart: Must be a valid UTC datetime in the future
    /// - SlotEnd: Must be after SlotStart
    /// 
    /// ## Example Request:
    /// ```json
    /// {
    ///   "dealershipId": "11111111-1111-1111-1111-111111111111",
    ///   "customerId": "22222222-2222-2222-2222-222222222222",
    ///   "vehicleId": "33333333-3333-3333-3333-333333333333",
    ///   "serviceTypeId": "44444444-4444-4444-4444-444444444444",
    ///   "technicianId": "55555555-5555-5555-5555-555555555555",
    ///   "serviceBayId": "66666666-6666-6666-6666-666666666666",
    ///   "slotStart": "2026-06-25T10:00:00Z",
    ///   "slotEnd": "2026-06-25T11:00:00Z"
    /// }
    /// ```
    /// 
    /// ## Example Success Response (201):
    /// ```json
    /// {
    ///   "appointmentId": "99999999-9999-9999-9999-999999999999",
    ///   "slotStart": "2026-06-25T10:00:00Z",
    ///   "slotEnd": "2026-06-25T11:00:00Z",
    ///   "createdAt": "2026-06-25T09:30:00Z"
    /// }
    /// ```
    /// 
    /// ## Error Responses:
    /// - **400 Bad Request**: Invalid input data or validation failure
    /// - **409 Conflict**: Selected time slot is no longer available or already booked
    /// - **500 Internal Server Error**: Unexpected server error
    /// </remarks>
    /// <param name="request">The appointment booking request containing all necessary details</param>
    /// <param name="idempotencyKey">Optional idempotency key supplied by the client in the <c>Idempotency-Key</c> header</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Created appointment with ID and timestamp</returns>
    /// <response code="201">Appointment created successfully</response>
    /// <response code="400">Invalid request data or validation failure</response>
    /// <response code="409">Service bay slot already booked or no longer available</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("appointments")]
    [ProducesResponseType(typeof(CreateAppointmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateAppointmentResponse>> CreateAppointment(
        [FromBody] CreateAppointmentRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey = null,
        CancellationToken cancellationToken = default)
    {
        Guid? idempotencyRecordId = null;

        try
        {
            // Validate request
            await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

            var idempotencyHandling = await _idempotencyRequestCoordinator
                .ValidateAndBeginCreateAppointmentAsync(Request, request, cancellationToken)
                .ConfigureAwait(false);

            if (idempotencyHandling.EarlyResponse != null)
            {
                return idempotencyHandling.EarlyResponse;
            }

            idempotencyRecordId = idempotencyHandling.RecordId;

            _logger.LogInformation(
                "Creating appointment: customerId={CustomerId}, vehicleId={VehicleId}, " +
                "technicianId={TechnicianId}, appointmentDate={AppointmentDate}, " +
                "startTimeSlotId={StartTimeSlotId}, endTimeSlotId={EndTimeSlotId}",
                request.CustomerId, request.VehicleId, request.TechnicianId,
                request.AppointmentDate, request.EstimatedStartTimeSlotId, request.EstimatedEndTimeSlotId);

            // Call service to create appointment
            var response = await _appointmentService.CreateAppointmentAsync(request, cancellationToken);

            _logger.LogInformation(
                "Appointment created successfully: appointmentId={AppointmentId}",
                response.AppointmentId);

            if (idempotencyRecordId.HasValue)
            {
                var body = JsonSerializer.Serialize(response, ReplayJsonOptions);
                await _idempotencyService
                    .CompleteRequestAsync(idempotencyRecordId.Value, StatusCodes.Status201Created, body, cancellationToken)
                    .ConfigureAwait(false);
            }

            return CreatedAtAction(
                nameof(GetAppointmentById),
                new { id = response.AppointmentId },
                response);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error in CreateAppointment");
            throw;  // Let middleware handle it
        }
        catch (BookingConflictException ex)
        {
            _logger.LogWarning(ex, "Booking conflict in CreateAppointment: {Message}", ex.Message);

            var response = new ErrorResponse
            {
                Message = "The selected slot is no longer available. Please check availability again.",
                ErrorCode = "BOOKING_CONFLICT",
                Timestamp = DateTime.UtcNow
            };

            if (idempotencyRecordId.HasValue)
            {
                var body = JsonSerializer.Serialize(response, ReplayJsonOptions);
                await _idempotencyService
                    .CompleteRequestAsync(idempotencyRecordId.Value, StatusCodes.Status409Conflict, body, cancellationToken)
                    .ConfigureAwait(false);
            }

            return Conflict(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation in CreateAppointment: {Message}", ex.Message);

            var response = new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "INVALID_OPERATION",
                Timestamp = DateTime.UtcNow
            };

            if (idempotencyRecordId.HasValue)
            {
                var body = JsonSerializer.Serialize(response, ReplayJsonOptions);
                await _idempotencyService
                    .CompleteRequestAsync(idempotencyRecordId.Value, StatusCodes.Status400BadRequest, body, cancellationToken)
                    .ConfigureAwait(false);
            }

            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in CreateAppointment");

            if (idempotencyRecordId.HasValue)
            {
                await _idempotencyService
                    .ReleaseRequestAsync(idempotencyRecordId.Value, cancellationToken)
                    .ConfigureAwait(false);
            }

            throw;  // Let middleware handle it
        }
    }

    /// <summary>
    /// Get appointment details by ID
    /// </summary>
    /// <remarks>
    /// Retrieves the full details of a specific appointment by its unique ID.
    /// 
    /// ## Path Parameters:
    /// - **id**: The GUID of the appointment to retrieve (must not be empty)
    /// 
    /// ## Example Request:
    /// ```
    /// GET /api/v1/appointments/99999999-9999-9999-9999-999999999999
    /// ```
    /// 
    /// ## Example Success Response (200):
    /// ```json
    /// {
    ///   "appointmentId": "99999999-9999-9999-9999-999999999999",
    ///   "slotStart": "2026-06-25T10:00:00Z",
    ///   "slotEnd": "2026-06-25T11:00:00Z",
    ///   "createdAt": "2026-06-25T09:30:00Z"
    /// }
    /// ```
    /// 
    /// ## Example Error Response (404):
    /// ```json
    /// {
    ///   "message": "Appointment with ID 99999999-9999-9999-9999-999999999999 not found",
    ///   "errorCode": "APPOINTMENT_NOT_FOUND",
    ///   "timestamp": "2026-06-25T09:30:00Z"
    /// }
    /// ```
    /// </remarks>
    /// <param name="id">The unique identifier (GUID) of the appointment to retrieve</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Appointment details if found</returns>
    /// <response code="200">Appointment found and returned</response>
    /// <response code="400">Invalid appointment ID (empty GUID)</response>
    /// <response code="404">Appointment not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("appointments/{id}")]
    [ProducesResponseType(typeof(CreateAppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateAppointmentResponse>> GetAppointmentById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Appointment ID cannot be empty",
                    ErrorCode = "INVALID_APPOINTMENT_ID",
                    Timestamp = DateTime.UtcNow
                });
            }

            _logger.LogInformation("Retrieving appointment: appointmentId={AppointmentId}", id);

            var appointment = await _appointmentService.GetAppointmentByIdAsync(id, cancellationToken);

            if (appointment == null)
            {
                return NotFound(new ErrorResponse
                {
                    Message = $"Appointment with ID {id} not found",
                    ErrorCode = "APPOINTMENT_NOT_FOUND",
                    Timestamp = DateTime.UtcNow
                });
            }

            return Ok(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetAppointmentById");
            throw;  // Let middleware handle it
        }
    }
}
