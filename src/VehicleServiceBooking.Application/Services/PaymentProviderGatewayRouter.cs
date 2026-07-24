using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VehicleServiceBooking.Application.Configuration;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Services;

/// <summary>
/// Routes payment-intent calls to real gateways when enabled, with optional development fallback.
/// </summary>
public sealed class PaymentProviderGatewayRouter : IPaymentProviderGateway
{
    private readonly PaymentProviderGatewayOptions _options;
    private readonly DevelopmentPaymentProviderGateway _developmentGateway;
    private readonly ZaloPayPaymentProviderGateway _zaloPayGateway;
    private readonly ILogger<PaymentProviderGatewayRouter> _logger;

    public PaymentProviderGatewayRouter(
        IOptions<PaymentProviderGatewayOptions> options,
        DevelopmentPaymentProviderGateway developmentGateway,
        ZaloPayPaymentProviderGateway zaloPayGateway,
        ILogger<PaymentProviderGatewayRouter> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _developmentGateway = developmentGateway ?? throw new ArgumentNullException(nameof(developmentGateway));
        _zaloPayGateway = zaloPayGateway ?? throw new ArgumentNullException(nameof(zaloPayGateway));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PaymentProviderIntentResult> CreateIntentAsync(
        PaymentProviderIntentRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.PaymentProviderType == PaymentProviderType.ZaloPay && _options.ZaloPay.Enabled)
        {
            return await _zaloPayGateway.CreateIntentAsync(request, cancellationToken).ConfigureAwait(false);
        }

        if (_options.UseDevelopmentGatewayFallback)
        {
            _logger.LogInformation(
                "Using development gateway fallback for provider={ProviderType}, method={MethodType}",
                request.PaymentProviderType,
                request.PaymentMethodType);

            return await _developmentGateway.CreateIntentAsync(request, cancellationToken).ConfigureAwait(false);
        }

        throw new InvalidOperationException(
            $"No real payment provider gateway is configured for provider '{request.PaymentProviderType}' and method '{request.PaymentMethodType}'.");
    }
}
