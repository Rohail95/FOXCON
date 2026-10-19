using BusinessLayer.Services;
using DatabaseLayer.Models; 
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EcommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController(IOrderService orderService) : BaseAdminController
    {
        public async Task<IActionResult> Index()
        {
            var orders = await orderService.GetAllOrdersAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await orderService.GetOrderByIdAsync(id.Value);
            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            var order = await orderService.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound();

            order.Status = status;

            await orderService.UpdateOrderAsync(order);

            return RedirectToAction("Details", new { id = orderId });
        }
    }
}