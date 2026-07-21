using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Interfaces.Services;

namespace VehicleServiceBooking.Application.Services;

/// <summary>
/// Provides payment status snapshots for orders.
/// </summary>
public sealed class PaymentStatusQueryService : IPaymentStatusQueryService
{
    private readonly IPaymentStatusQueryRepository _paymentStatusQueryRepository;

    public PaymentStatusQueryService(IPaymentStatusQueryRepository paymentStatusQueryRepository)
    {
        _paymentStatusQueryRepository = paymentStatusQueryRepository ?? throw new ArgumentNullException(nameof(paymentStatusQueryRepository));
    }

    public async Task<GetPaymentStatusResponse?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _paymentStatusQueryRepository
            .GetOrderPaymentSnapshotByIdAsync(orderId, cancellationToken)
            .ConfigureAwait(false);

        if (order == null)
        {
            return null;
        }

        if (order.PaymentStatus == null)
        {
            throw new InvalidOperationException($"Order '{orderId}' is missing payment status lookup linkage.");
        }

        var response = new GetPaymentStatusResponse
        {
            OrderId = order.Id,
            PaymentStatusId = order.PaymentStatusId,
            OrderPaymentStatus = order.PaymentStatus.Name,
            TotalAmount = order.TotalAmount,
            AmountPaid = order.AmountPaid,
            PaidAtUtc = order.PaidAtUtc
        };

        if (order.PaymentOrder != null)
        {
            response.PaymentOrder = new PaymentOrderStatusSummary
            {
                PaymentOrderId = order.PaymentOrder.Id,
                PaymentProviderId = order.PaymentOrder.PaymentProviderId,
                PaymentMethodId = order.PaymentOrder.PaymentMethodId,
                StatusId = order.PaymentOrder.StatusId,
                PaymentIntentStatus = order.PaymentOrder.Status?.Name ?? string.Empty,
                IntentCode = order.PaymentOrder.IntentCode,
                CheckoutUrl = order.PaymentOrder.CheckoutUrl,
                ExpiresAtUtc = order.PaymentOrder.ExpiresAtUtc,
                OrderedAtUtc = order.PaymentOrder.OrderedAtUtc,
                LastProviderEventAtUtc = order.PaymentOrder.LastProviderEventAtUtc,
                LastProviderEventId = order.PaymentOrder.LastProviderEventId
            };

            var transaction = order.PaymentOrder.PaymentTransaction;
            if (transaction != null)
            {
                response.PaymentTransaction = new PaymentTransactionStatusSummary
                {
                    PaymentTransactionId = transaction.Id,
                    TransactionCode = transaction.TransactionCode,
                    PaymentProviderId = transaction.PaymentProviderId,
                    PaymentMethodId = transaction.PaymentMethodId,
                    StatusId = transaction.StatusId,
                    PaymentTransactionStatus = transaction.Status?.Name ?? string.Empty,
                    Amount = transaction.Amount,
                    TransactionAtUtc = transaction.TransactionAtUtc,
                    CompletedAtUtc = transaction.CompletedAtUtc,
                    FailedAtUtc = transaction.FailedAtUtc,
                    ProviderTransactionId = transaction.ProviderTransactionId,
                    ProviderEventId = transaction.ProviderEventId
                };
            }
        }

        return response;
    }
}
