using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using VehicleServiceBooking.Application.Exceptions;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for order aggregate persistence operations.
/// </summary>
public sealed class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private const string UniqueOrderCodeIndex = "IX_Order_OrderCode_Unique";
    private const string UniqueAppointmentOrderIndex = "IX_OrderAppointment_Appointment_Unique";

    public OrderRepository(IApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<Order?> GetAggregateByIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await DbContext.Orders
            .AsNoTracking()
            .Include(x => x.ServiceTypeOrders)
            .Include(x => x.OrderAppointments)
            .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> OrderCodeExistsAsync(string orderCode, CancellationToken cancellationToken)
    {
        return await GetQueryable()
            .AnyAsync(x => x.OrderCode == orderCode, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> AnyAppointmentAlreadyLinkedAsync(
        IEnumerable<Guid> appointmentIds,
        CancellationToken cancellationToken)
    {
        var appointmentIdList = appointmentIds.Distinct().ToList();
        if (appointmentIdList.Count == 0)
        {
            return false;
        }

        return await DbContext.OrderAppointments
            .AsNoTracking()
            .AnyAsync(x => appointmentIdList.Contains(x.AppointmentId), cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Order> CreateAggregateAsync(
        Order order,
        IReadOnlyCollection<ServiceTypeOrder> serviceTypeOrders,
        IReadOnlyCollection<OrderAppointment> orderAppointments,
        CancellationToken cancellationToken)
    {
        try
        {
            DbContext.Orders.Add(order);

            if (serviceTypeOrders.Count > 0)
            {
                DbContext.ServiceTypeOrders.AddRange(serviceTypeOrders);
            }

            if (orderAppointments.Count > 0)
            {
                DbContext.OrderAppointments.AddRange(orderAppointments);
            }

            await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return order;
        }
        catch (DbUpdateException ex) when (IsOrderConflictViolation(ex))
        {
            DbContext.DbContext.ChangeTracker.Clear();

            throw new OrderConflictException(
                "Order creation conflict detected due to duplicate order code or appointment linkage.",
                ex);
        }
    }

    private static bool IsOrderConflictViolation(DbUpdateException ex)
    {
        if (ex.InnerException is PostgresException pgEx)
        {
            var isUniqueViolation = pgEx.SqlState == PostgresErrorCodes.UniqueViolation;
            var isKnownConstraint = string.Equals(pgEx.ConstraintName, UniqueOrderCodeIndex, StringComparison.Ordinal) ||
                                    string.Equals(pgEx.ConstraintName, UniqueAppointmentOrderIndex, StringComparison.Ordinal);

            return isUniqueViolation && isKnownConstraint;
        }

        var rawMessage = ex.ToString();
        return rawMessage.Contains(UniqueOrderCodeIndex, StringComparison.Ordinal) ||
               rawMessage.Contains(UniqueAppointmentOrderIndex, StringComparison.Ordinal);
    }
}
