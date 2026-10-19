using BusinessLayer.Services;
using DatabaseLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EcommerceWeb.Controllers
{
    [Authorize] // IMPORTANT: This entire controller is now only for logged-in users.
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        // GET: /Order/ or /Order/History
        // Shows a list of all orders placed by the current user.
        public async Task<IActionResult> History()
        {
            var userId = _userManager.GetUserId(User);
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return View("~/Views/Order/History.cshtml", orders);
        }

        // GET: /Order/Details/5
        // Shows the details for a single order.
        public async Task<IActionResult> Details(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return NotFound(); // Order doesn't exist.
            }

            // SECURITY CHECK: Make sure the user is only viewing their own order.
            var userId = _userManager.GetUserId(User);
            if (order.UserId != userId)
            {
                return Forbid(); // The user is not authorized to see this order.
            }

            return View("~/Views/Order/Details.cshtml", order);
        }
    }
}