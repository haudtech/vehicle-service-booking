using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Application.Interfaces.Services;

namespace VehicleServiceBooking.Application.Services;

/// <summary>
/// Development-safe payment provider gateway that returns deterministic checkout links.
/// </summary>
public sealed class DevelopmentPaymentProviderGateway : IPaymentProviderGateway
{
    public Task<PaymentProviderIntentResult> CreateIntentAsync(
        PaymentProviderIntentRequest request,
        CancellationToken cancellationToken = default)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(15);
        var amount = request.Amount.ToString("0.00", CultureInfo.InvariantCulture);

        var result = new PaymentProviderIntentResult
        {
            CheckoutUrl = $"https://payments.local/checkout/{request.IntentCode}?amount={amount}&orderId={request.OrderId}",
            ExpiresAtUtc = expiresAtUtc
        };

        return Task.FromResult(result);
    }
}
