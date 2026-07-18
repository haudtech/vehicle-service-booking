namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Commercial order aggregate identified by a unique business order code.
/// </summary>
public class Order : BaseEntity
{
    public string OrderCode { get; set; } = string.Empty;

    public ICollection<ServiceTypeOrder> ServiceTypeOrders { get; set; } = new List<ServiceTypeOrder>();

    public PaymentOrder? PaymentOrder { get; set; }

    public ICollection<OrderAppointment> OrderAppointments { get; set; } = new List<OrderAppointment>();
}
