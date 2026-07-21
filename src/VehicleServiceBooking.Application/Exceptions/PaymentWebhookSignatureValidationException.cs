using System;

namespace VehicleServiceBooking.Application.Exceptions;

/// <summary>
/// Represents a webhook signature validation failure.
/// </summary>
public sealed class PaymentWebhookSignatureValidationException : UnauthorizedAccessException
{
    public PaymentWebhookSignatureValidationException(string message)
        : base(message)
    {
    }

    public PaymentWebhookSignatureValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}