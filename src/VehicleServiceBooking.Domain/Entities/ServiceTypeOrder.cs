namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Junction table between Order and ServiceType.
/// Order total can be derived from linked service type prices.
/// </summary>
public class ServiceTypeOrder : BaseEntity
{
    public Guid OrderId { get; set; }

    public Guid ServiceTypeId { get; set; }

    public int Quantity { get; set; } = 1;

    public Order Order { get; set; } = null!;

    public ServiceType ServiceType { get; set; } = null!;
}
