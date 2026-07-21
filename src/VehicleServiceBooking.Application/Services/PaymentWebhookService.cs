using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VehicleServiceBooking.Application.Configuration;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Exceptions;
using VehicleServiceBooking.Application.Helpers;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Services;

/// <summary>
/// Processes provider webhook events with idempotent inbox dedupe.
/// </summary>
public sealed class PaymentWebhookService : IPaymentWebhookService
{
    private readonly IPaymentWebhookRepository _paymentWebhookRepository;
    private readonly PaymentWebhookSecurityOptions _webhookSecurityOptions;
    private readonly ILogger<PaymentWebhookService> _logger;

    public PaymentWebhookService(
        IPaymentWebhookRepository paymentWebhookRepository,
        IOptions<PaymentWebhookSecurityOptions> webhookSecurityOptions,
        ILogger<PaymentWebhookService> logger)
    {
        _paymentWebhookRepository = paymentWebhookRepository ?? throw new ArgumentNullException(nameof(paymentWebhookRepository));
        _webhookSecurityOptions = webhookSecurityOptions?.Value ?? throw new ArgumentNullException(nameof(webhookSecurityOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProcessPaymentWebhookResponse> ProcessAsync(
        PaymentProviderType providerType,
        ProcessPaymentWebhookRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.EventId))
        {
            throw new InvalidOperationException("Webhook eventId is required.");
        }

        if (string.IsNullOrWhiteSpace(request.IntentCode))
        {
            throw new InvalidOperationException("Webhook intentCode is required.");
        }

        VerifySignature(providerType, request);

        var providerId = await _paymentWebhookRepository
            .GetActivePaymentProviderIdAsync(providerType, cancellationToken)
            .ConfigureAwait(false);

        if (!providerId.HasValue)
        {
            throw new InvalidOperationException($"Payment provider '{providerType}' is not configured or inactive.");
        }

        var normalizedEventId = request.EventId.Trim();
        var existingInbox = await _paymentWebhookRepository
            .GetInboxByProviderAndEventIdAsync(providerId.Value, normalizedEventId, cancellationToken)
            .ConfigureAwait(false);

        if (existingInbox != null)
        {
            _logger.LogInformation(
                "Duplicate webhook ignored: provider={ProviderType}, eventId={EventId}, inboxId={InboxId}",
                providerType,
                normalizedEventId,
                existingInbox.Id);

            return new ProcessPaymentWebhookResponse
            {
                WebhookInboxId = existingInbox.Id,
                EventId = normalizedEventId,
                IsDuplicate = true,
                ProcessStatus = existingInbox.ProcessStatus?.Name ?? "Received",
                Message = "Webhook event already processed or queued."
            };
        }

        var receivedStatus = await _paymentWebhookRepository
            .GetWebhookProcessStatusAsync(PaymentWebhookProcessStatus.Received, cancellationToken)
            .ConfigureAwait(false);

        var utcNow = DateTime.UtcNow;
        var webhookInbox = new PaymentWebhookInbox
        {
            Id = Guid.NewGuid(),
            PaymentProviderId = providerId.Value,
            EventId = normalizedEventId,
            SignatureHash = request.SignatureHash?.Trim() ?? string.Empty,
            Payload = request.Payload?.Trim() ?? string.Empty,
            ProcessStatusId = receivedStatus.Id,
            ReceivedAtUtc = request.OccurredAtUtc ?? utcNow,
            CreatedAt = utcNow,
            UpdatedAt = utcNow,
            IsActive = true
        };

        await _paymentWebhookRepository
            .AddWebhookInboxAsync(webhookInbox, cancellationToken)
            .ConfigureAwait(false);

        var paymentOrder = await _paymentWebhookRepository
            .GetPaymentOrderForWebhookAsync(providerId.Value, request.IntentCode.Trim(), cancellationToken)
            .ConfigureAwait(false);

        if (paymentOrder == null)
        {
            var ignoredStatus = await _paymentWebhookRepository
                .GetWebhookProcessStatusAsync(PaymentWebhookProcessStatus.Ignored, cancellationToken)
                .ConfigureAwait(false);

            webhookInbox.ProcessStatusId = ignoredStatus.Id;
            webhookInbox.ProcessedAtUtc = utcNow;
            webhookInbox.ErrorMessage = "No payment order found for the provided intent code.";
            webhookInbox.UpdatedAt = utcNow;

            await _paymentWebhookRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new ProcessPaymentWebhookResponse
            {
                WebhookInboxId = webhookInbox.Id,
                EventId = normalizedEventId,
                IsDuplicate = false,
                ProcessStatus = ignoredStatus.Name,
                Message = "Webhook was accepted but ignored because no payment order matched intentCode."
            };
        }

        if (paymentOrder.PaymentTransaction == null)
        {
            var failedStatus = await _paymentWebhookRepository
                .GetWebhookProcessStatusAsync(PaymentWebhookProcessStatus.Failed, cancellationToken)
                .ConfigureAwait(false);

            webhookInbox.ProcessStatusId = failedStatus.Id;
            webhookInbox.ProcessedAtUtc = utcNow;
            webhookInbox.ErrorMessage = "Matched payment order has no linked payment transaction.";
            webhookInbox.UpdatedAt = utcNow;

            await _paymentWebhookRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new ProcessPaymentWebhookResponse
            {
                WebhookInboxId = webhookInbox.Id,
                EventId = normalizedEventId,
                IsDuplicate = false,
                ProcessStatus = failedStatus.Name,
                Message = "Webhook processing failed because payment transaction linkage is missing."
            };
        }

        if (!Enum.TryParse<PaymentTransactionStatus>(request.TransactionStatus, true, out var transactionStatus))
        {
            throw new InvalidOperationException($"Unsupported transactionStatus '{request.TransactionStatus}'.");
        }

        var transactionStatusLookup = await _paymentWebhookRepository
            .GetPaymentTransactionStatusAsync(transactionStatus, cancellationToken)
            .ConfigureAwait(false);

        var intentStatusTarget = MapIntentStatus(transactionStatus);
        var orderPaymentStatusTarget = MapOrderPaymentStatus(transactionStatus);

        var intentStatusLookup = await _paymentWebhookRepository
            .GetPaymentIntentStatusAsync(intentStatusTarget, cancellationToken)
            .ConfigureAwait(false);

        var orderPaymentStatusLookup = await _paymentWebhookRepository
            .GetOrderPaymentStatusAsync(orderPaymentStatusTarget, cancellationToken)
            .ConfigureAwait(false);

        paymentOrder.StatusId = intentStatusLookup.Id;
        paymentOrder.LastProviderEventId = normalizedEventId;
        paymentOrder.LastProviderEventAtUtc = request.OccurredAtUtc ?? utcNow;
        paymentOrder.UpdatedAt = utcNow;

        var paymentTransaction = paymentOrder.PaymentTransaction;
        paymentTransaction.StatusId = transactionStatusLookup.Id;
        paymentTransaction.ProviderEventId = normalizedEventId;
        paymentTransaction.ProviderTransactionId = request.ProviderTransactionId?.Trim();
        paymentTransaction.RawProviderPayload = request.Payload;
        paymentTransaction.UpdatedAt = utcNow;

        paymentTransaction.CompletedAtUtc = transactionStatus == PaymentTransactionStatus.Completed
            ? (request.OccurredAtUtc ?? utcNow)
            : paymentTransaction.CompletedAtUtc;

        paymentTransaction.FailedAtUtc = transactionStatus is PaymentTransactionStatus.Failed or PaymentTransactionStatus.Cancelled or PaymentTransactionStatus.Expired
            ? (request.OccurredAtUtc ?? utcNow)
            : paymentTransaction.FailedAtUtc;

        paymentOrder.Order.PaymentStatusId = orderPaymentStatusLookup.Id;
        paymentOrder.Order.UpdatedAt = utcNow;

        if (transactionStatus == PaymentTransactionStatus.Completed)
        {
            paymentOrder.Order.AmountPaid = paymentOrder.Order.TotalAmount;
            paymentOrder.Order.PaidAtUtc = request.OccurredAtUtc ?? utcNow;
        }

        if (transactionStatus is PaymentTransactionStatus.Failed or PaymentTransactionStatus.Cancelled or PaymentTransactionStatus.Expired)
        {
            paymentOrder.Order.PaidAtUtc = null;
        }

        var processedStatus = await _paymentWebhookRepository
            .GetWebhookProcessStatusAsync(PaymentWebhookProcessStatus.Processed, cancellationToken)
            .ConfigureAwait(false);

        webhookInbox.ProcessStatusId = processedStatus.Id;
        webhookInbox.ProcessedAtUtc = utcNow;
        webhookInbox.ErrorMessage = null;
        webhookInbox.UpdatedAt = utcNow;

        await _paymentWebhookRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation(
            "Processed webhook: provider={ProviderType}, eventId={EventId}, intentCode={IntentCode}, txStatus={TxStatus}",
            providerType,
            normalizedEventId,
            request.IntentCode,
            transactionStatus.ToString());

        return new ProcessPaymentWebhookResponse
        {
            WebhookInboxId = webhookInbox.Id,
            EventId = normalizedEventId,
            IsDuplicate = false,
            ProcessStatus = processedStatus.Name,
            Message = "Webhook processed successfully."
        };
    }

    private void VerifySignature(PaymentProviderType providerType, ProcessPaymentWebhookRequest request)
    {
        if (!_webhookSecurityOptions.Enabled || !_webhookSecurityOptions.RequireSignature)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_webhookSecurityOptions.SharedSecret))
        {
            throw new InvalidOperationException("Payment webhook signature secret is not configured.");
        }

        if (string.IsNullOrWhiteSpace(request.SignatureHash))
        {
            throw new PaymentWebhookSignatureValidationException("Webhook signature is required.");
        }

        var canonicalMessage = PaymentWebhookSignatureHelper.BuildCanonicalMessage(providerType, request);
        var expectedSignature = PaymentWebhookSignatureHelper.ComputeSignature(_webhookSecurityOptions.SharedSecret, canonicalMessage);

        var providedBytes = Encoding.UTF8.GetBytes(request.SignatureHash.Trim().ToUpperInvariant());
        var expectedBytes = Encoding.UTF8.GetBytes(expectedSignature.ToUpperInvariant());

        if (providedBytes.Length != expectedBytes.Length ||
            !CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes))
        {
            throw new PaymentWebhookSignatureValidationException("Webhook signature is invalid.");
        }
    }

    private static PaymentIntentStatus MapIntentStatus(PaymentTransactionStatus transactionStatus)
    {
        return transactionStatus switch
        {
            PaymentTransactionStatus.Completed => PaymentIntentStatus.Paid,
            PaymentTransactionStatus.Failed => PaymentIntentStatus.Failed,
            PaymentTransactionStatus.Expired => PaymentIntentStatus.Expired,
            PaymentTransactionStatus.Cancelled => PaymentIntentStatus.Cancelled,
            _ => PaymentIntentStatus.Redirected
        };
    }

    private static OrderPaymentStatus MapOrderPaymentStatus(PaymentTransactionStatus transactionStatus)
    {
        return transactionStatus switch
        {
            PaymentTransactionStatus.Completed => OrderPaymentStatus.Paid,
            PaymentTransactionStatus.Failed => OrderPaymentStatus.Failed,
            PaymentTransactionStatus.Expired => OrderPaymentStatus.Expired,
            PaymentTransactionStatus.Cancelled => OrderPaymentStatus.Cancelled,
            PaymentTransactionStatus.Refunded => OrderPaymentStatus.Refunded,
            _ => OrderPaymentStatus.Pending
        };
    }
}