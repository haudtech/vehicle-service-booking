using System;

namespace VehicleServiceBooking.Application.DTOs;

/// <summary>
/// Aggregated payment status response for an order.
/// </summary>
public sealed class GetPaymentStatusResponse
{
    public Guid OrderId { get; set; }

    public Guid PaymentStatusId { get; set; }

    public string OrderPaymentStatus { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public decimal AmountPaid { get; set; }

    public DateTime? PaidAtUtc { get; set; }

    public PaymentOrderStatusSummary? PaymentOrder { get; set; }

    public PaymentTransactionStatusSummary? PaymentTransaction { get; set; }
}

/// <summary>
/// Payment-order-level status summary.
/// </summary>
public sealed class PaymentOrderStatusSummary
{
    public Guid PaymentOrderId { get; set; }

    public Guid PaymentProviderId { get; set; }

    public Guid PaymentMethodId { get; set; }

    public Guid StatusId { get; set; }

    public string PaymentIntentStatus { get; set; } = string.Empty;

    public string IntentCode { get; set; } = string.Empty;

    public string? CheckoutUrl { get; set; }

    public DateTime? ExpiresAtUtc { get; set; }

    public DateTime OrderedAtUtc { get; set; }

    public DateTime? LastProviderEventAtUtc { get; set; }

    public string? LastProviderEventId { get; set; }
}

/// <summary>
/// Payment-transaction-level status summary.
/// </summary>
public sealed class PaymentTransactionStatusSummary
{
    public Guid PaymentTransactionId { get; set; }

    public string TransactionCode { get; set; } = string.Empty;

    public Guid PaymentProviderId { get; set; }

    public Guid PaymentMethodId { get; set; }

    public Guid StatusId { get; set; }

    public string PaymentTransactionStatus { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime TransactionAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public DateTime? FailedAtUtc { get; set; }

    public string? ProviderTransactionId { get; set; }

    public string? ProviderEventId { get; set; }
}
