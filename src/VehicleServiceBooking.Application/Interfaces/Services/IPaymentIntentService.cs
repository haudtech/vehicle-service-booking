using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Application.DTOs;

namespace VehicleServiceBooking.Application.Interfaces.Services;

/// <summary>
/// Service for creating and reusing payment intents tied to orders.
/// </summary>
public interface IPaymentIntentService
{
    Task<CreatePaymentIntentResponse> CreatePaymentIntentAsync(
        Guid orderId,
        CreatePaymentIntentRequest request,
        CancellationToken cancellationToken = default);
}
