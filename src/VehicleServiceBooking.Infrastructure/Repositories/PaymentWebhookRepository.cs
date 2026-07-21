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
/// Repository implementation for payment webhook processing data access.
/// </summary>
public sealed class PaymentWebhookRepository : GenericRepository<PaymentWebhookInbox>, IPaymentWebhookRepository
{
    public PaymentWebhookRepository(IApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<Guid?> GetActivePaymentProviderIdAsync(
        PaymentProviderType providerType,
        CancellationToken cancellationToken)
    {
        var provider = await GetQueryable<PaymentProviderLookup>()
            .SingleOrDefaultAsync(
                x => x.Provider == providerType && x.IsActive,
                cancellationToken)
            .ConfigureAwait(false);

        return provider?.Id;
    }

    public async Task<PaymentWebhookInbox?> GetInboxByProviderAndEventIdAsync(
        Guid paymentProviderId,
        string eventId,
        CancellationToken cancellationToken)
    {
        return await GetQueryable()
            .Include(x => x.ProcessStatus)
            .SingleOrDefaultAsync(
                x => x.PaymentProviderId == paymentProviderId && x.EventId == eventId,
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<PaymentWebhookProcessStatusLookup> GetWebhookProcessStatusAsync(
        PaymentWebhookProcessStatus status,
        CancellationToken cancellationToken)
    {
        return await GetQueryable<PaymentWebhookProcessStatusLookup>()
            .SingleAsync(x => x.Status == status, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<PaymentOrder?> GetPaymentOrderForWebhookAsync(
        Guid paymentProviderId,
        string intentCode,
        CancellationToken cancellationToken)
    {
        return await GetQueryable<PaymentOrder>(asNoTracking: false)
            .Include(x => x.Order)
            .Include(x => x.PaymentTransaction)
            .SingleOrDefaultAsync(
                x => x.PaymentProviderId == paymentProviderId && x.IntentCode == intentCode,
                cancellationToken)
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

    public async Task<OrderPaymentStatusLookup> GetOrderPaymentStatusAsync(
        OrderPaymentStatus status,
        CancellationToken cancellationToken)
    {
        return await GetQueryable<OrderPaymentStatusLookup>()
            .SingleAsync(x => x.Status == status, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task AddWebhookInboxAsync(PaymentWebhookInbox webhookInbox, CancellationToken cancellationToken)
    {
        await AddAsync(webhookInbox, cancellationToken).ConfigureAwait(false);
    }
}