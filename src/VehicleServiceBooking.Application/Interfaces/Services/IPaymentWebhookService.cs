using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Interfaces.Services;

/// <summary>
/// Service for processing inbound payment provider webhook events.
/// </summary>
public interface IPaymentWebhookService
{
    Task<ProcessPaymentWebhookResponse> ProcessAsync(
        PaymentProviderType providerType,
        ProcessPaymentWebhookRequest request,
        CancellationToken cancellationToken = default);
}