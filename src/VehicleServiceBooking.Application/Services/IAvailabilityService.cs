using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Application.Models;

namespace VehicleServiceBooking.Application.Services;

public interface IAvailabilityService
{
    Task<List<TimeSlot>> GetAvailableSlotsAsync(
        Guid dealershipId,
        Guid serviceTypeId,
        DateTime date,
        CancellationToken cancellationToken = default);

}