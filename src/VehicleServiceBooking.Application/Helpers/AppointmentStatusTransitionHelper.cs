using System.Collections.Generic;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Helpers;

/// <summary>
/// Centralizes business rules for appointment lifecycle state transitions.
/// </summary>
public static class AppointmentStatusTransitionHelper
{
    private static readonly IReadOnlyDictionary<AppointmentStatus, IReadOnlySet<AppointmentStatus>> AllowedTransitions =
        new Dictionary<AppointmentStatus, IReadOnlySet<AppointmentStatus>>
        {
            [AppointmentStatus.Booked] = new HashSet<AppointmentStatus> { AppointmentStatus.Cancelled },
            [AppointmentStatus.InProgress] = new HashSet<AppointmentStatus> { AppointmentStatus.Cancelled, AppointmentStatus.Completed },
            [AppointmentStatus.PartiallyCompleted] = new HashSet<AppointmentStatus> { AppointmentStatus.Completed }
        };

    public static bool CanTransitionTo(AppointmentStatus currentStatus, AppointmentStatus requestedStatus)
    {
        return AllowedTransitions.TryGetValue(currentStatus, out var allowedTargets) && allowedTargets.Contains(requestedStatus);
    }

    public static string GetInvalidTransitionMessage(AppointmentStatus currentStatus, AppointmentStatus requestedStatus)
    {
        return requestedStatus switch
        {
            AppointmentStatus.Cancelled => $"Appointment cannot be cancelled from status '{currentStatus}'. Allowed statuses: Booked, InProgress.",
            AppointmentStatus.Completed => $"Appointment cannot be completed from status '{currentStatus}'. Allowed statuses: InProgress, PartiallyCompleted.",
            _ => $"Appointment cannot transition to status '{requestedStatus}' from '{currentStatus}'."
        };
    }
}
