using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWeb.ViewModels
{
    public class ProductCreateViewModel
    {
        [Required]
        public string Name { get; set; } = "";

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative number.")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public int? CategoryId { get; set; }

        [Required]
        public string Sku { get; set; } = ""; 

        public IFormFile? ImageFile { get; set; }
    }
}