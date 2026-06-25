using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Interfaces;

namespace VehicleServiceBooking.Api.Controllers;

/// <summary>
/// Manages availability queries for appointment booking
/// </summary>
/// <remarks>
/// This controller provides real-time availability information for service appointments.
/// It helps clients discover open time slots for specific services at specific dealerships.
/// </remarks>
[ApiController]
[Route("api/v1")]
[Produces("application/json")]
[Tags("Availability")]
public class AvailabilityController : ControllerBase
{
    private readonly IAvailabilityService _availabilityService;
    private readonly IValidator<GetAvailabilityRequest> _validator;
    private readonly ILogger<AvailabilityController> _logger;

    public AvailabilityController(
        IAvailabilityService availabilityService,
        IValidator<GetAvailabilityRequest> validator,
        ILogger<AvailabilityController> logger)
    {
        _availabilityService = availabilityService ?? throw new ArgumentNullException(nameof(availabilityService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get available appointment slots for a service
    /// </summary>
    /// <remarks>
    /// Queries the system for available time slots on a specific date for a given service type.
    /// 
    /// This endpoint performs real-time availability checking and returns:
    /// - All available time slots for the specified service
    /// - Assigned technician and service bay for each slot
    /// - A rank score indicating the quality of each option (based on technician skills and bay suitability)
    /// 
    /// ## Query Parameters:
    /// - **dealershipId** (required): GUID of the dealership to query
    /// - **serviceTypeId** (required): GUID of the service type to search for
    /// - **date** (required): The date to search for (YYYY-MM-DD format)
    /// 
    /// ## Example Request:
    /// ```
    /// GET /api/v1/availability?dealershipId=11111111-1111-1111-1111-111111111111&amp;serviceTypeId=44444444-4444-4444-4444-444444444444&amp;date=2026-06-25
    /// ```
    /// 
    /// ## Example Success Response (200):
    /// ```json
    /// [
    ///   {
    ///     "slotStart": "2026-06-25T09:00:00Z",
    ///     "slotEnd": "2026-06-25T10:00:00Z",
    ///     "technicianId": "55555555-5555-5555-5555-555555555555",
    ///     "serviceBayId": "66666666-6666-6666-6666-666666666666",
    ///     "rankScore": 85
    ///   },
    ///   {
    ///     "slotStart": "2026-06-25T10:00:00Z",
    ///     "slotEnd": "2026-06-25T11:00:00Z",
    ///     "technicianId": "77777777-7777-7777-7777-777777777777",
    ///     "serviceBayId": "66666666-6666-6666-6666-666666666666",
    ///     "rankScore": 72
    ///   }
    /// ]
    /// ```
    /// 
    /// ## Empty Response Example (200):
    /// When no slots are available, returns an empty array:
    /// ```json
    /// []
    /// ```
    /// </remarks>
    /// <param name="request">Query parameters for availability search (dealershipId, serviceTypeId, date)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>List of available slots with technician and bay assignments</returns>
    /// <response code="200">Returns list of available slots (may be empty if none available)</response>
    /// <response code="400">Invalid parameters or validation failed</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("availability")]
    [ProducesResponseType(typeof(List<AvailabilityOptionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<AvailabilityOptionResponse>>> GetAvailableSlots(
        [FromQuery] GetAvailabilityRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate request
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _logger.LogInformation(
                "Querying availability: dealershipId={DealershipId}, serviceTypeId={ServiceTypeId}, date={Date}",
                request.DealershipId, request.ServiceTypeId, request.Date);

            // Get available slots from service
            var options = await _availabilityService.GetAvailableSlotsAsync(
                request.DealershipId, request.ServiceTypeId, request.Date, cancellationToken);

            // Convert to response DTOs
            var response = options
                .Select(o => new AvailabilityOptionResponse
                {
                    SlotStart = o.TimeSlot.Start,
                    SlotEnd = o.TimeSlot.End,
                    TechnicianId = o.TechnicianId,
                    ServiceBayId = o.ServiceBayId,
                    RankScore = 0  // TODO: Implement ranking in Phase 15.24
                })
                .ToList();

            _logger.LogInformation(
                "Availability query completed: found {Count} options",
                response.Count);

            return Ok(response);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error in GetAvailableSlots");
            throw;  // Let middleware handle it
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation in GetAvailableSlots: {Message}", ex.Message);
            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "INVALID_OPERATION",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetAvailableSlots");
            throw;  // Let middleware handle it
        }
    }
}
