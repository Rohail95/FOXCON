using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DatabaseLayer.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    [Required(ErrorMessage = "Please select a category.")]
    public int? CategoryId { get; set; }

    public string Sku { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = [];

    public virtual Category? Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = [];

    public virtual ICollection<Review> Reviews { get; set; } = [];
    public double AverageRating { get; set; }
    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}
