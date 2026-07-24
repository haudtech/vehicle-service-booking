using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Application.DTOs;

namespace VehicleServiceBooking.Application.Interfaces.Services;

/// <summary>
/// Normalizes a provider-native ZaloPay callback into the internal webhook request model.
/// </summary>
public interface IZaloPayWebhookIngressService
{
    Task<ProcessPaymentWebhookRequest> NormalizeAsync(
        JsonElement payload,
        CancellationToken cancellationToken = default);
}