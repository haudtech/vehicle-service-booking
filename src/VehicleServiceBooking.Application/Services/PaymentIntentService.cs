using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Services;

/// <summary>
/// Creates or reuses payment intents for orders.
/// </summary>
public sealed class PaymentIntentService : IPaymentIntentService
{
    private readonly IPaymentIntentRepository _paymentIntentRepository;
    private readonly IPaymentProviderGateway _paymentProviderGateway;
    private readonly ILogger<PaymentIntentService> _logger;

    public PaymentIntentService(
        IPaymentIntentRepository paymentIntentRepository,
        IPaymentProviderGateway paymentProviderGateway,
        ILogger<PaymentIntentService> logger)
    {
        _paymentIntentRepository = paymentIntentRepository ?? throw new ArgumentNullException(nameof(paymentIntentRepository));
        _paymentProviderGateway = paymentProviderGateway ?? throw new ArgumentNullException(nameof(paymentProviderGateway));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CreatePaymentIntentResponse> CreatePaymentIntentAsync(
        Guid orderId,
        CreatePaymentIntentRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = await _paymentIntentRepository
            .GetOrderForIntentAsync(orderId, cancellationToken)
            .ConfigureAwait(false);

        if (order == null)
        {
            throw new InvalidOperationException($"Order '{orderId}' was not found or is inactive.");
        }

        if (order.PaymentStatus == null)
        {
            throw new InvalidOperationException($"Order '{orderId}' is missing payment status lookup linkage.");
        }

        if (order.PaymentStatus.Status == OrderPaymentStatus.Paid ||
            order.PaymentStatus.Status == OrderPaymentStatus.Cancelled ||
            order.PaymentStatus.Status == OrderPaymentStatus.Refunded)
        {
            throw new InvalidOperationException(
                $"Order '{orderId}' cannot create payment intent in status '{order.PaymentStatus.Status}'.");
        }

        var providerExists = await _paymentIntentRepository
            .PaymentProviderExistsAsync(request.PaymentProviderId, cancellationToken)
            .ConfigureAwait(false);
        if (!providerExists)
        {
            throw new InvalidOperationException($"Payment provider '{request.PaymentProviderId}' was not found or is inactive.");
        }

        var methodExists = await _paymentIntentRepository
            .PaymentMethodExistsAsync(request.PaymentMethodId, cancellationToken)
            .ConfigureAwait(false);
        if (!methodExists)
        {
            throw new InvalidOperationException($"Payment method '{request.PaymentMethodId}' was not found or is inactive.");
        }

        var utcNow = DateTime.UtcNow;

        if (CanReuseExistingIntent(order, request, utcNow))
        {
            var existingPaymentOrder = order.PaymentOrder!;
            _logger.LogInformation(
                "Reusing active payment intent: orderId={OrderId}, paymentOrderId={PaymentOrderId}, intentCode={IntentCode}",
                order.Id,
                existingPaymentOrder.Id,
                existingPaymentOrder.IntentCode);

            var existingTransaction = existingPaymentOrder.PaymentTransaction;
            if (existingTransaction == null)
            {
                throw new InvalidOperationException(
                    $"Payment order '{existingPaymentOrder.Id}' is missing linked transaction for active intent reuse.");
            }

            return new CreatePaymentIntentResponse
            {
                OrderId = order.Id,
                PaymentOrderId = existingPaymentOrder.Id,
                PaymentTransactionId = existingTransaction.Id,
                PaymentProviderId = existingPaymentOrder.PaymentProviderId,
                PaymentMethodId = existingPaymentOrder.PaymentMethodId,
                IntentCode = existingPaymentOrder.IntentCode,
                CheckoutUrl = existingPaymentOrder.CheckoutUrl,
                ExpiresAtUtc = existingPaymentOrder.ExpiresAtUtc,
                OrderPaymentStatus = order.PaymentStatus.Name,
                PaymentIntentStatus = existingPaymentOrder.Status.Name,
                PaymentTransactionStatus = existingTransaction.Status.Name,
                IsExistingIntent = true
            };
        }

        var pendingOrderPaymentStatus = await _paymentIntentRepository
            .GetOrderPaymentStatusAsync(OrderPaymentStatus.Pending, cancellationToken)
            .ConfigureAwait(false);

        var initiatedIntentStatus = await _paymentIntentRepository
            .GetPaymentIntentStatusAsync(PaymentIntentStatus.Initiated, cancellationToken)
            .ConfigureAwait(false);

        var pendingTransactionStatus = await _paymentIntentRepository
            .GetPaymentTransactionStatusAsync(PaymentTransactionStatus.Pending, cancellationToken)
            .ConfigureAwait(false);

        var intentCode = GenerateIntentCode();
        var transactionCode = GenerateTransactionCode();

        var providerResult = await _paymentProviderGateway
            .CreateIntentAsync(
                new PaymentProviderIntentRequest
                {
                    OrderId = order.Id,
                    IntentCode = intentCode,
                    Amount = order.TotalAmount,
                    CurrencyId = order.CurrencyId,
                    PaymentProviderId = request.PaymentProviderId,
                    PaymentMethodId = request.PaymentMethodId
                },
                cancellationToken)
            .ConfigureAwait(false);

        var paymentTransaction = new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            TransactionCode = transactionCode,
            PaymentProviderId = request.PaymentProviderId,
            PaymentMethodId = request.PaymentMethodId,
            FromAccount = "customer",
            ToAccount = "merchant",
            Direction = PaymentTransactionDirection.InCome,
            Amount = order.TotalAmount,
            CurrencyId = order.CurrencyId,
            StatusId = pendingTransactionStatus.Id,
            TransactionAtUtc = utcNow,
            CreatedAt = utcNow,
            UpdatedAt = utcNow,
            IsActive = true
        };

        PaymentOrder? newPaymentOrder = null;

        if (order.PaymentOrder == null)
        {
            newPaymentOrder = new PaymentOrder
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                PaymentProviderId = request.PaymentProviderId,
                PaymentMethodId = request.PaymentMethodId,
                StatusId = initiatedIntentStatus.Id,
                IntentCode = intentCode,
                CheckoutUrl = providerResult.CheckoutUrl,
                ExpiresAtUtc = providerResult.ExpiresAtUtc,
                LastProviderEventId = null,
                LastProviderEventAtUtc = null,
                PaymentTransactionId = paymentTransaction.Id,
                OrderedAtUtc = utcNow,
                CreatedAt = utcNow,
                UpdatedAt = utcNow,
                IsActive = true
            };

            order.PaymentOrder = newPaymentOrder;
        }
        else
        {
            order.PaymentOrder.PaymentProviderId = request.PaymentProviderId;
            order.PaymentOrder.PaymentMethodId = request.PaymentMethodId;
            order.PaymentOrder.StatusId = initiatedIntentStatus.Id;
            order.PaymentOrder.IntentCode = intentCode;
            order.PaymentOrder.CheckoutUrl = providerResult.CheckoutUrl;
            order.PaymentOrder.ExpiresAtUtc = providerResult.ExpiresAtUtc;
            order.PaymentOrder.LastProviderEventId = null;
            order.PaymentOrder.LastProviderEventAtUtc = null;
            order.PaymentOrder.PaymentTransactionId = paymentTransaction.Id;
            order.PaymentOrder.OrderedAtUtc = utcNow;
            order.PaymentOrder.UpdatedAt = utcNow;
        }

        order.PaymentStatusId = pendingOrderPaymentStatus.Id;
        order.AmountPaid = 0m;
        order.PaidAtUtc = null;
        order.UpdatedAt = utcNow;

        await _paymentIntentRepository
            .PersistIntentAsync(paymentTransaction, newPaymentOrder, cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation(
            "Created payment intent: orderId={OrderId}, paymentOrderId={PaymentOrderId}, paymentTransactionId={PaymentTransactionId}, intentCode={IntentCode}",
            order.Id,
            order.PaymentOrder!.Id,
            paymentTransaction.Id,
            intentCode);

        return new CreatePaymentIntentResponse
        {
            OrderId = order.Id,
            PaymentOrderId = order.PaymentOrder!.Id,
            PaymentTransactionId = paymentTransaction.Id,
            PaymentProviderId = order.PaymentOrder.PaymentProviderId,
            PaymentMethodId = order.PaymentOrder.PaymentMethodId,
            IntentCode = order.PaymentOrder.IntentCode,
            CheckoutUrl = order.PaymentOrder.CheckoutUrl,
            ExpiresAtUtc = order.PaymentOrder.ExpiresAtUtc,
            OrderPaymentStatus = pendingOrderPaymentStatus.Name,
            PaymentIntentStatus = initiatedIntentStatus.Name,
            PaymentTransactionStatus = pendingTransactionStatus.Name,
            IsExistingIntent = false
        };
    }

    private static bool CanReuseExistingIntent(
        Order order,
        CreatePaymentIntentRequest request,
        DateTime utcNow)
    {
        var paymentOrder = order.PaymentOrder;
        if (paymentOrder == null || paymentOrder.Status == null)
        {
            return false;
        }

        var isOpenIntentStatus = paymentOrder.Status.Status == PaymentIntentStatus.Initiated ||
                                 paymentOrder.Status.Status == PaymentIntentStatus.Redirected;

        if (!isOpenIntentStatus)
        {
            return false;
        }

        var notExpired = !paymentOrder.ExpiresAtUtc.HasValue || paymentOrder.ExpiresAtUtc.Value > utcNow;
        var sameRoute = paymentOrder.PaymentProviderId == request.PaymentProviderId &&
                        paymentOrder.PaymentMethodId == request.PaymentMethodId;

        return notExpired && sameRoute;
    }

    private static string GenerateIntentCode()
    {
        return $"PI-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..36];
    }

    private static string GenerateTransactionCode()
    {
        return $"TX-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..38];
    }
}
