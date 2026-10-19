using System;
using System.Collections.Generic;

namespace DatabaseLayer.Models;


public partial class Shipping
{
    public int ShippingId { get; set; }

    public int OrderId { get; set; }

    public string? TrackingNumber { get; set; }

    public string Carrier { get; set; } = null!;

    public string? Status { get; set; }

    public DateOnly? EstimatedDelivery { get; set; }

    public DateOnly? ActualDelivery { get; set; }

    public decimal ShippingCost { get; set; }

    public virtual Order Order { get; set; } = null!;
}
