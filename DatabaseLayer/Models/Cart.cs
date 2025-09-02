using System;
using System.Collections.Generic;

namespace DatabaseLayer.Models
{
    public partial class Cart
    {
        public int CartId { get; set; }

        
        public required string UserId { get; set; }
        public required virtual ApplicationUser User { get; set; } 

        
        public int? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}