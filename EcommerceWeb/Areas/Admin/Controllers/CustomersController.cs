using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomersController(ICustomerService customerService) : BaseAdminController
    {
        
        public async Task<IActionResult> Index()
        {
            var customers = await customerService.GetAllCustomersAsync();
            return View(customers);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var customer = await customerService.GetCustomerWithOrdersAsync(id.Value);
            if (customer == null) return NotFound();

            return View(customer);
        }
    }
}