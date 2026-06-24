using VehicleServiceBooking.Application.Models;

namespace VehicleServiceBooking.Application.Services;

public interface IAvailabilityService
{
    List<TimeSlot> GetAvailableSlots(
        Guid dealershipId,
        Guid serviceTypeId,
        DateTime date);
}