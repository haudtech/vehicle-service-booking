using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Order aggregate persistence operations.
/// </summary>
public interface IOrderRepository : IReadRepository<Order>, IWriteRepository<Order>
{
    Task<Order?> GetAggregateByIdAsync(Guid orderId, CancellationToken cancellationToken);

    Task<bool> OrderCodeExistsAsync(string orderCode, CancellationToken cancellationToken);

    Task<bool> AnyAppointmentAlreadyLinkedAsync(
        IEnumerable<Guid> appointmentIds,
        CancellationToken cancellationToken);

    Task<OrderPaymentStatusLookup?> GetOrderPaymentStatusAsync(
        OrderPaymentStatus status,
        CancellationToken cancellationToken);

    Task<Order> CreateAggregateAsync(
        Order order,
        IReadOnlyCollection<ServiceTypeOrder> serviceTypeOrders,
        IReadOnlyCollection<OrderAppointment> orderAppointments,
        CancellationToken cancellationToken);
}