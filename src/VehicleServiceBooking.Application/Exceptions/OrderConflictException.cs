using System;

namespace VehicleServiceBooking.Application.Exceptions;

/// <summary>
/// Represents a conflict when creating or mutating an order.
/// </summary>
public sealed class OrderConflictException : InvalidOperationException
{
    public OrderConflictException(string message)
        : base(message)
    {
    }

    public OrderConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}