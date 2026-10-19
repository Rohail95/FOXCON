using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
namespace DatabaseLayer.Models
{
    public partial class Order
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        public required string ShippingAddress { get; set; }

        public required string BillingAddress { get; set; }
        public required string PaymentMethod { get; set; }
       
        public string? Notes { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }


        public virtual Customer Customer { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();
    }
}