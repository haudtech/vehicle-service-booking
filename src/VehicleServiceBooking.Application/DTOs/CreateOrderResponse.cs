using System;
using System.Collections.Generic;

namespace VehicleServiceBooking.Application.DTOs;

/// <summary>
/// Response payload after creating an order.
/// </summary>
public sealed class CreateOrderResponse
{
    public Guid OrderId { get; set; }

    public string OrderCode { get; set; } = string.Empty;

    public Guid CurrencyId { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<CreateOrderServiceTypeItemResponse> ServiceTypeItems { get; set; }
        = new List<CreateOrderServiceTypeItemResponse>();

    public ICollection<Guid> AppointmentIds { get; set; } = new List<Guid>();
}

/// <summary>
/// Service type line item response in order creation response.
/// </summary>
public sealed class CreateOrderServiceTypeItemResponse
{
    public Guid ServiceTypeId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }
}