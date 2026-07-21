using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository contract for payment-status query data access.
/// </summary>
public interface IPaymentStatusQueryRepository
{
    Task<Order?> GetOrderPaymentSnapshotByIdAsync(Guid orderId, CancellationToken cancellationToken);
}