using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository contract for payment webhook data access and persistence.
/// </summary>
public interface IPaymentWebhookRepository
{
    Task<Guid?> GetActivePaymentProviderIdAsync(
        PaymentProviderType providerType,
        CancellationToken cancellationToken);

    Task<PaymentWebhookInbox?> GetInboxByProviderAndEventIdAsync(
        Guid paymentProviderId,
        string eventId,
        CancellationToken cancellationToken);

    Task<PaymentWebhookProcessStatusLookup> GetWebhookProcessStatusAsync(
        PaymentWebhookProcessStatus status,
        CancellationToken cancellationToken);

    Task<PaymentOrder?> GetPaymentOrderForWebhookAsync(
        Guid paymentProviderId,
        string intentCode,
        CancellationToken cancellationToken);

    Task<PaymentIntentStatusLookup> GetPaymentIntentStatusAsync(
        PaymentIntentStatus status,
        CancellationToken cancellationToken);

    Task<PaymentTransactionStatusLookup> GetPaymentTransactionStatusAsync(
        PaymentTransactionStatus status,
        CancellationToken cancellationToken);

    Task<OrderPaymentStatusLookup> GetOrderPaymentStatusAsync(
        OrderPaymentStatus status,
        CancellationToken cancellationToken);

    Task AddWebhookInboxAsync(PaymentWebhookInbox webhookInbox, CancellationToken cancellationToken);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);

    Task<T> ExecuteInExecutionStrategyAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken);

    void ClearChangeTracker();

    Task SaveChangesAsync(CancellationToken cancellationToken);
}