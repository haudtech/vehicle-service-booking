using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VehicleServiceBooking.Api.Common.Utils;
using VehicleServiceBooking.Api.Services;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Exceptions;
using VehicleServiceBooking.Application.Interfaces.Services;

namespace VehicleServiceBooking.Api.Controllers;

/// <summary>
/// Manages order creation for service bookings.
/// </summary>
[ApiController]
[Route("api/v1")]
[Produces("application/json")]
[Tags("Orders")]
public sealed class OrdersController : ControllerBase
{
    private static readonly JsonSerializerOptions ReplayJsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IValidator<CreateOrderRequest> _createValidator;
    private readonly IOrderService _orderService;
    private readonly IIdempotencyService _idempotencyService;
    private readonly IIdempotencyRequestCoordinator _idempotencyRequestCoordinator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IValidator<CreateOrderRequest> createValidator,
        IOrderService orderService,
        IIdempotencyService idempotencyService,
        IIdempotencyRequestCoordinator idempotencyRequestCoordinator,
        ILogger<OrdersController> logger)
    {
        _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        _idempotencyService = idempotencyService ?? throw new ArgumentNullException(nameof(idempotencyService));
        _idempotencyRequestCoordinator = idempotencyRequestCoordinator ?? throw new ArgumentNullException(nameof(idempotencyRequestCoordinator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves an order by its unique identifier.
    /// </summary>
    [HttpGet("orders/{orderId:guid}")]
    [Authorize(Policy = "OrderViewPolicy")]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateOrderResponse>> GetOrderById(
        [FromRoute] Guid orderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (orderId == Guid.Empty)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Order ID cannot be empty",
                    ErrorCode = "INVALID_ORDER_ID",
                    Timestamp = DateTime.UtcNow
                });
            }

            var response = await _orderService
                .GetOrderByIdAsync(orderId, cancellationToken)
                .ConfigureAwait(false);

            if (response == null)
            {
                return NotFound(new ErrorResponse
                {
                    Message = $"Order with ID {orderId} not found",
                    ErrorCode = "ORDER_NOT_FOUND",
                    Timestamp = DateTime.UtcNow
                });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetOrderById: orderId={OrderId}", orderId);
            throw;
        }
    }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <remarks>
    /// Creates a new commercial order with one or more service type line items,
    /// and optional appointment links.
    ///
    /// The endpoint performs the following checks:
    /// - validates request payload and cross-field constraints
    /// - enforces unique orderCode
    /// - validates referenced serviceType and appointment entities
    /// - prevents linking an appointment already attached to another order
    /// - handles idempotency replay/conflict behavior when Idempotency-Key is supplied
    ///
    /// ## Request Requirements:
    /// - Authorization: Bearer token with permission claim order:create
    /// - Idempotency-Key: optional unless configured as required
    /// - orderCode: required, max length 50, unique
    /// - currencyId: required, must be an active supported currency
    /// - serviceTypeItems: required, at least one item
    /// - appointmentIds: optional, must not contain duplicates
    ///
    /// ## Example Request:
    /// ```json
    /// {
    ///   "orderCode": "ORD-2026-1001",
    ///   "currencyId": "00000000-0000-0000-0004-000000000001",
    ///   "serviceTypeItems": [
    ///     {
    ///       "serviceTypeId": "11111111-1111-1111-1111-030000000001",
    ///       "quantity": 1
    ///     }
    ///   ],
    ///   "appointmentIds": [
    ///     "99999999-9999-9999-9999-999999999999"
    ///   ]
    /// }
    /// ```
    ///
    /// ## Example Success Response (201):
    /// ```json
    /// {
    ///   "orderId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
    ///   "orderCode": "ORD-2026-1001",
    ///   "currencyId": "00000000-0000-0000-0004-000000000001",
    ///   "totalAmount": 100.0,
    ///   "createdAt": "2026-07-18T08:00:00Z",
    ///   "serviceTypeItems": [
    ///     {
    ///       "serviceTypeId": "11111111-1111-1111-1111-030000000001",
    ///       "quantity": 1,
    ///       "unitPrice": 100.0,
    ///       "lineTotal": 100.0
    ///     }
    ///   ],
    ///   "appointmentIds": [
    ///     "99999999-9999-9999-9999-999999999999"
    ///   ]
    /// }
    /// ```
    /// </remarks>
    [HttpPost("orders")]
    [Authorize(Policy = "OrderCreatePolicy")]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateOrderResponse>> CreateOrder(
        [FromBody] CreateOrderRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey = null,
        CancellationToken cancellationToken = default)
    {
        Guid? idempotencyRecordId = null;

        try
        {
            if (string.IsNullOrWhiteSpace(request.OrderCode))
            {
                request.OrderCode = OrderCodeGenerator.GenerateOrderCode();
            }

            await _createValidator.ValidateAndThrowAsync(request, cancellationToken)
                .ConfigureAwait(false);

            var idempotencyHandling = await _idempotencyRequestCoordinator
                .ValidateAndBeginCreateOrderAsync(Request, request, cancellationToken)
                .ConfigureAwait(false);

            if (idempotencyHandling.EarlyResponse != null)
            {
                return idempotencyHandling.EarlyResponse;
            }

            idempotencyRecordId = idempotencyHandling.RecordId;

            _logger.LogInformation(
                "Creating order: orderCode={OrderCode}, serviceTypeItemCount={ServiceTypeItemCount}, appointmentCount={AppointmentCount}",
                request.OrderCode,
                request.ServiceTypeItems.Count,
                request.AppointmentIds.Count);

            var response = await _orderService
                .CreateOrderAsync(request, cancellationToken)
                .ConfigureAwait(false);

            if (idempotencyRecordId.HasValue)
            {
                var body = JsonSerializer.Serialize(response, ReplayJsonOptions);
                await _idempotencyService
                    .CompleteRequestAsync(idempotencyRecordId.Value, StatusCodes.Status201Created, body, cancellationToken)
                    .ConfigureAwait(false);
            }

            return Created($"/api/v1/orders/{response.OrderId}", response);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (OrderConflictException ex)
        {
            _logger.LogWarning(ex, "Order conflict in CreateOrder: {Message}", ex.Message);

            var response = new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "ORDER_CONFLICT",
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
            _logger.LogWarning(ex, "Invalid operation in CreateOrder: {Message}", ex.Message);

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
            _logger.LogError(ex, "Unexpected error in CreateOrder");

            if (idempotencyRecordId.HasValue)
            {
                await _idempotencyService
                    .ReleaseRequestAsync(idempotencyRecordId.Value, cancellationToken)
                    .ConfigureAwait(false);
            }

            throw;
        }
    }
}
