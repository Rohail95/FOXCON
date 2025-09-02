

using DatabaseLayer.Models;
using System.Collections.Generic; 

namespace EcommerceWeb.Models
{
    public class DashboardViewModel
    {
      
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalStockQuantity { get; set; }
        public int LowStockProductsCount { get; set; }
        public List<Product> LowStockProducts { get; set; } = new List<Product>();
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalCustomers { get; set; }
        public int NewCustomersLast30Days { get; set; }
    
    }
}