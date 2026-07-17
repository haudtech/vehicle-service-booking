using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Application.DTOs;

namespace VehicleServiceBooking.Application.Interfaces.Services;

/// <summary>
/// Resolves core user profile data from Auth service.
/// </summary>
public interface IAuthUserProfileClient
{
    Task<AuthUserCoreProfileDto?> GetUserCoreProfileByIdAsync(Guid authUserId, CancellationToken cancellationToken = default);
}