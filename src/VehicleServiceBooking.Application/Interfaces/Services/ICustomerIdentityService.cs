using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Services;

/// <summary>
/// Provides customer identity resolution from auth user identity.
/// </summary>
public interface ICustomerIdentityService
{
    Task<Customer> GetOrCreateCustomerByAuthUserIdAsync(Guid authUserId, CancellationToken cancellationToken = default);
}