using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
        await AddWithoutSaveAsync(webhookInbox, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        var providerName = DbContext.DbContext.Database.ProviderName;
        // EF InMemory does not support real transactions; return a no-op transaction
        // so the service flow can keep a single transaction code path in tests.
        if (providerName?.Contains("InMemory", StringComparison.OrdinalIgnoreCase) == true)
        {
            return new NoopDbContextTransaction();
        }

        // Relational providers use a real database transaction boundary.
        return await DbContext.DbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
    }

    public void ClearChangeTracker()
    {
        DbContext.DbContext.ChangeTracker.Clear();
    }

    // Minimal IDbContextTransaction implementation used only when the provider
    // cannot open transactions (for example EF InMemory in integration tests).
    private sealed class NoopDbContextTransaction : IDbContextTransaction
    {
        public Guid TransactionId { get; } = Guid.NewGuid();

        public void Commit()
        {
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Rollback()
        {
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}