using System;

namespace VehicleServiceBooking.Application.DTOs;

/// <summary>
/// Represents customer profile data returned to API consumers.
/// </summary>
public sealed class CustomerProfileResponse
{
    public Guid CustomerId { get; set; }

    public Guid AuthUserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}