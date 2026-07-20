using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Application.DTOs;

namespace VehicleServiceBooking.Application.Interfaces.Services;

/// <summary>
/// Service for managing order operations.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Creates a new order.
    /// </summary>
    Task<CreateOrderResponse> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an order by its identifier.
    /// </summary>
    Task<CreateOrderResponse?> GetOrderByIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);
}