using System;

namespace VehicleServiceBooking.Application.DTOs;

/// <summary>
/// Minimal core profile returned by Auth service for customer bootstrap.
/// </summary>
public sealed class AuthUserCoreProfileDto
{
    public Guid AuthUserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsEmailVerified { get; set; }

    public bool IsActive { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}