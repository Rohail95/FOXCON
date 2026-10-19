using BusinessLayer.Services;
using EcommerceWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System; // Required for DateTime
using System.Linq;

namespace EcommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : BaseAdminController
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;

       
        public HomeController(
            IProductService productService,
            ICategoryService categoryService,
            ICustomerService customerService,
              IOrderService orderService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _customerService = customerService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var allProducts = await _productService.GetAllProductsAsync();
            var allCategories = await _categoryService.GetAllCategoriesAsync();
            var allCustomers = await _customerService.GetAllCustomersAsync();
            var allOrders = await _orderService.GetAllOrdersAsync();

            const int lowStockThreshold = 10;
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            var validOrders = allOrders.Where(o => o.Status != "Cancelled").ToList();

            var viewModel = new DashboardViewModel
            {
                TotalProducts = allProducts.Count(),
                TotalCategories = allCategories.Count(),
                TotalStockQuantity = allProducts.Sum(p => p.StockQuantity),
                LowStockProductsCount = allProducts.Count(p => p.StockQuantity < lowStockThreshold),
                LowStockProducts = allProducts.Where(p => p.StockQuantity < lowStockThreshold).ToList(),

                TotalCustomers = allCustomers.Count(),
                NewCustomersLast30Days = allCustomers.Count(c => c.CreatedAt >= thirtyDaysAgo),

                 TotalRevenue = validOrders.Sum(o => o.TotalAmount),
                AverageOrderValue = validOrders.Any() ? validOrders.Average(o => o.TotalAmount) : 0
            };

            return View(viewModel);
        }
    }
}