using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Static lookup table for payment methodologies/channels.
/// </summary>
public class PaymentMethodLookup : BaseEntity
{
    public PaymentMethodType Method { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
