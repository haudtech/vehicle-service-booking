using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository contract for payment-intent-specific data access and persistence.
/// </summary>
public interface IPaymentIntentRepository
{
    Task<Order?> GetOrderForIntentAsync(Guid orderId, CancellationToken cancellationToken);

    Task<PaymentOrder?> GetPaymentOrderByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);

    Task<bool> PaymentProviderExistsAsync(Guid paymentProviderId, CancellationToken cancellationToken);

    Task<bool> PaymentMethodExistsAsync(Guid paymentMethodId, CancellationToken cancellationToken);

    Task<PaymentProviderLookup?> GetPaymentProviderAsync(Guid paymentProviderId, CancellationToken cancellationToken);

    Task<PaymentMethodLookup?> GetPaymentMethodAsync(Guid paymentMethodId, CancellationToken cancellationToken);

    Task<OrderPaymentStatusLookup> GetOrderPaymentStatusAsync(
        OrderPaymentStatus status,
        CancellationToken cancellationToken);

    Task<PaymentIntentStatusLookup> GetPaymentIntentStatusAsync(
        PaymentIntentStatus status,
        CancellationToken cancellationToken);

    Task<PaymentTransactionStatusLookup> GetPaymentTransactionStatusAsync(
        PaymentTransactionStatus status,
        CancellationToken cancellationToken);

    Task PersistIntentAsync(
        PaymentTransaction paymentTransaction,
        PaymentOrder? newPaymentOrder,
        CancellationToken cancellationToken);
}