namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Commercial order aggregate identified by a unique business order code.
/// Ownership is system/business scoped and intentionally not tied directly to Customer.
/// Customer context is derived through linked appointments when required.
/// </summary>
public class Order : BaseEntity
{
    public string OrderCode { get; set; } = string.Empty;

    public Guid CurrencyId { get; set; }

    public decimal TotalAmount { get; set; }

    public CurrencyLookup Currency { get; set; } = null!;

    public ICollection<ServiceTypeOrder> ServiceTypeOrders { get; set; } = new List<ServiceTypeOrder>();

    public PaymentOrder? PaymentOrder { get; set; }

    public ICollection<OrderAppointment> OrderAppointments { get; set; } = new List<OrderAppointment>();
}
