using System;

namespace VehicleServiceBooking.Application.Exceptions;

/// <summary>
/// Represents a booking conflict where the requested slot/resources
/// are no longer available at commit time or re-validation time.
/// </summary>
public sealed class BookingConflictException : InvalidOperationException
{
    public BookingConflictException()
        : base("The selected slot is no longer available. Please check availability again.")
    {
    }

    public BookingConflictException(string message)
        : base(message)
    {
    }

    public BookingConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
