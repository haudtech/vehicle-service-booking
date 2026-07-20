using System;
using System.Collections.Generic;

namespace VehicleServiceBooking.Application.DTOs;

/// <summary>
/// Request payload to create a commercial order.
/// </summary>
public sealed class CreateOrderRequest
{
    public string OrderCode { get; set; } = string.Empty;

    public Guid CurrencyId { get; set; }

    public ICollection<CreateOrderServiceTypeItemRequest> ServiceTypeItems { get; set; }
        = new List<CreateOrderServiceTypeItemRequest>();

    public ICollection<Guid> AppointmentIds { get; set; } = new List<Guid>();
}

/// <summary>
/// Service type line item request for order creation.
/// </summary>
public sealed class CreateOrderServiceTypeItemRequest
{
    public Guid ServiceTypeId { get; set; }

    public int Quantity { get; set; }
}