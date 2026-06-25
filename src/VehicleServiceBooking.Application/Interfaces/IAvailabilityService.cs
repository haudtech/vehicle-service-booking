using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Application.Models;

namespace VehicleServiceBooking.Application.Interfaces;

public interface IAvailabilityService
{
    Task<List<AvailabilityOption>> GetAvailableSlotsAsync(
        Guid dealershipId,
        Guid serviceTypeId,
        DateTime date,
        CancellationToken cancellationToken = default);

}
