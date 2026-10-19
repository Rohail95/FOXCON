using DatabaseLayer.Models;

namespace EcommerceWeb.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; } = new();

        public bool IsOnSale { get; set; }

        public decimal DisplayPrice { get; set; }

        public decimal? OriginalPrice { get; set; }
    }
}