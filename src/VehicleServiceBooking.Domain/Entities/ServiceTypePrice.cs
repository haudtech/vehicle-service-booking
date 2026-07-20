namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Junction table that defines the selling price of a service type in a specific currency.
/// </summary>
public class ServiceTypePrice : BaseEntity
{
    public Guid ServiceTypeId { get; set; }

    public Guid CurrencyId { get; set; }

    public decimal Price { get; set; }

    public ServiceType ServiceType { get; set; } = null!;

    public CurrencyLookup Currency { get; set; } = null!;
}
