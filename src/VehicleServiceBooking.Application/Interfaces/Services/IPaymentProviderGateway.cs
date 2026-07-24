using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Interfaces.Services;

/// <summary>
/// Provider gateway abstraction for creating checkout intents.
/// </summary>
public interface IPaymentProviderGateway
{
    Task<PaymentProviderIntentResult> CreateIntentAsync(
        PaymentProviderIntentRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Input contract for provider-side intent creation.
/// </summary>
public sealed class PaymentProviderIntentRequest
{
    public Guid OrderId { get; set; }

    public string IntentCode { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public Guid CurrencyId { get; set; }

    public Guid PaymentProviderId { get; set; }

    public Guid PaymentMethodId { get; set; }

    public PaymentProviderType PaymentProviderType { get; set; }

    public PaymentMethodType PaymentMethodType { get; set; }
}

/// <summary>
/// Output contract from provider-side intent creation.
/// </summary>
public sealed class PaymentProviderIntentResult
{
    public string CheckoutUrl { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }
}
