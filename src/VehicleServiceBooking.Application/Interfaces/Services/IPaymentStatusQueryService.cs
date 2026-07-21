using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Application.DTOs;

namespace VehicleServiceBooking.Application.Interfaces.Services;

/// <summary>
/// Service for querying payment status snapshots by order.
/// </summary>
public interface IPaymentStatusQueryService
{
    Task<GetPaymentStatusResponse?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);
}
