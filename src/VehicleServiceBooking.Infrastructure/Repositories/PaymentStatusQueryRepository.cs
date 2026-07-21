using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for payment-status query snapshots.
/// </summary>
public sealed class PaymentStatusQueryRepository : GenericRepository<Order>, IPaymentStatusQueryRepository
{
    public PaymentStatusQueryRepository(IApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<Order?> GetOrderPaymentSnapshotByIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await GetQueryable()
            .Include(x => x.PaymentStatus)
            .Include(x => x.PaymentOrder)
                .ThenInclude(x => x!.Status)
            .Include(x => x.PaymentOrder)
                .ThenInclude(x => x!.PaymentTransaction)
                    .ThenInclude(x => x!.Status)
            .SingleOrDefaultAsync(x => x.Id == orderId, cancellationToken)
            .ConfigureAwait(false);
    }
}