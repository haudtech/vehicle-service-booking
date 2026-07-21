using System;

namespace VehicleServiceBooking.Application.DTOs;

/// <summary>
/// Request payload to create or reuse a payment intent for an existing order.
/// </summary>
public sealed class CreatePaymentIntentRequest
{
    public Guid PaymentProviderId { get; set; }

    public Guid PaymentMethodId { get; set; }
}

/// <summary>
/// Response payload for payment intent creation.
/// </summary>
public sealed class CreatePaymentIntentResponse
{
    public Guid OrderId { get; set; }

    public Guid PaymentOrderId { get; set; }

    public Guid PaymentTransactionId { get; set; }

    public Guid PaymentProviderId { get; set; }

    public Guid PaymentMethodId { get; set; }

    public string IntentCode { get; set; } = string.Empty;

    public string? CheckoutUrl { get; set; }

    public DateTime? ExpiresAtUtc { get; set; }

    public string OrderPaymentStatus { get; set; } = string.Empty;

    public string PaymentIntentStatus { get; set; } = string.Empty;

    public string PaymentTransactionStatus { get; set; } = string.Empty;

    public bool IsExistingIntent { get; set; }
}
