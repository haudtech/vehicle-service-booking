using VehicleServiceBooking.Application.Models;

namespace VehicleServiceBooking.Application.Services;

public class AvailabilityService : IAvailabilityService
{
    public List<TimeSlot> GetAvailableSlots(
        Guid dealershipId,
        Guid serviceTypeId,
        DateTime date)
    {
        // STEP 1: Get service duration (mock)
        int durationMinutes = GetServiceDuration(serviceTypeId);

        // STEP 2: Get working hours window (mock)
        var (start, end) = GetWorkingHoursWindow(date);

        // STEP 3: Generate raw slots
        var slots = GenerateTimeSlots(start, end, durationMinutes);

        return slots;
    }

    // MOCK: Service duration
    private int GetServiceDuration(Guid serviceTypeId)
    {
        return 60;
    }

    // MOCK: Working hours
    private (TimeSpan Start, TimeSpan End) GetWorkingHoursWindow(DateTime date)
    {
        return (
            new TimeSpan(8, 0, 0),
            new TimeSpan(17, 0, 0)
        );
    }

    // STEP 3: Generate slots
    private List<TimeSlot> GenerateTimeSlots(
        TimeSpan start,
        TimeSpan end,
        int durationMinutes)
    {
        var result = new List<TimeSlot>();

        var current = start;

        while (current.Add(TimeSpan.FromMinutes(durationMinutes)) <= end)
        {
            var slot = new TimeSlot
            {
                Start = DateTime.Today.Add(current),
                End = DateTime.Today.Add(current.Add(TimeSpan.FromMinutes(durationMinutes)))
            };

            result.Add(slot);

            current = current.Add(TimeSpan.FromMinutes(durationMinutes));
        }

        return result;
    }
}