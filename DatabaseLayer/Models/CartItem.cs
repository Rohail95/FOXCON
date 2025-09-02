using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseLayer.Models
{
    public partial class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        public int CartId { get; set; }
        public virtual  Cart Cart { get; set; } = null!;

        public int ProductId { get; set; }
        public virtual  Product Product { get; set; } = null!;

        public int Quantity { get; set; }

     
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}