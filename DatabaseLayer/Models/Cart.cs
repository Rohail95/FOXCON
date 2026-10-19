using System;
using System.Collections.Generic;

namespace DatabaseLayer.Models
{
    public partial class Cart
    {
        public int CartId { get; set; }

        
        public  string? UserId { get; set; }
        public  virtual ApplicationUser? User { get; set; } 

        
        public int? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public string? AppliedDiscountCode { get; set; }
        public decimal DiscountAmount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? SessionId { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}