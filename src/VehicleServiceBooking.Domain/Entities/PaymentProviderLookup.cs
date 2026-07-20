using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Static lookup table for supported payment providers.
/// </summary>
public class PaymentProviderLookup : BaseEntity
{
    public PaymentProviderType Provider { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
