using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for payment-intent-specific queries and persistence.
/// </summary>
public sealed class PaymentIntentRepository : GenericRepository<Order>, IPaymentIntentRepository
{
    public PaymentIntentRepository(IApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<Order?> GetOrderForIntentAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await GetQueryable(asNoTracking: false)
            .Include(x => x.PaymentStatus)
            .Include(x => x.PaymentOrder)
                .ThenInclude(x => x!.Status)
            .Include(x => x.PaymentOrder)
                .ThenInclude(x => x!.PaymentTransaction)
                    .ThenInclude(x => x!.Status)
            .SingleOrDefaultAsync(x => x.Id == orderId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<PaymentOrder?> GetPaymentOrderByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await GetQueryable<PaymentOrder>(asNoTracking: false)
            .IgnoreQueryFilters()
            .Include(x => x.Status)
            .Include(x => x.PaymentTransaction)
                .ThenInclude(x => x!.Status)
            .SingleOrDefaultAsync(x => x.OrderId == orderId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> PaymentProviderExistsAsync(Guid paymentProviderId, CancellationToken cancellationToken)
    {
        return await GetQueryable<PaymentProviderLookup>()
            .AnyAsync(x => x.Id == paymentProviderId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> PaymentMethodExistsAsync(Guid paymentMethodId, CancellationToken cancellationToken)
    {
        return await GetQueryable<PaymentMethodLookup>()
            .AnyAsync(x => x.Id == paymentMethodId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<PaymentProviderLookup?> GetPaymentProviderAsync(Guid paymentProviderId, CancellationToken cancellationToken)
    {
        return await GetQueryable<PaymentProviderLookup>()
            .SingleOrDefaultAsync(x => x.Id == paymentProviderId && x.IsActive, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<PaymentMethodLookup?> GetPaymentMethodAsync(Guid paymentMethodId, CancellationToken cancellationToken)
    {
        return await GetQueryable<PaymentMethodLookup>()
            .SingleOrDefaultAsync(x => x.Id == paymentMethodId && x.IsActive, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<OrderPaymentStatusLookup> GetOrderPaymentStatusAsync(
        OrderPaymentStatus status,
        CancellationToken cancellationToken)
    {
        return await GetQueryable<OrderPaymentStatusLookup>()
            .SingleAsync(x => x.Status == status, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<PaymentIntentStatusLookup> GetPaymentIntentStatusAsync(
        PaymentIntentStatus status,
        CancellationToken cancellationToken)
    {
        return await GetQueryable<PaymentIntentStatusLookup>()
            .SingleAsync(x => x.Status == status, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<PaymentTransactionStatusLookup> GetPaymentTransactionStatusAsync(
        PaymentTransactionStatus status,
        CancellationToken cancellationToken)
    {
        return await GetQueryable<PaymentTransactionStatusLookup>()
            .SingleAsync(x => x.Status == status, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task PersistIntentAsync(
        PaymentTransaction paymentTransaction,
        PaymentOrder? newPaymentOrder,
        CancellationToken cancellationToken)
    {
        AddEntity(paymentTransaction);

        if (newPaymentOrder != null)
        {
            AddEntity(newPaymentOrder);
        }

        await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}