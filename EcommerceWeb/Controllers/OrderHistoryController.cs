using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DatabaseLayer.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace EcommerceWeb.Controllers
{
    [Authorize]
    public class OrderHistoryController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderHistoryController(IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string status = "all")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var allOrders = await _orderService.GetOrdersByUserIdAsync(user.Id);
            IEnumerable<Order> filteredOrders;

            ViewData["ActiveStatusFilter"] = status;

            switch (status.ToLower())
            {
                case "completed":
                    filteredOrders = allOrders.Where(o =>
                        o.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase));
                    break;

                case "toreview":
                    filteredOrders = allOrders.Where(o =>
                        o.Status.Equals("Shipped", StringComparison.OrdinalIgnoreCase) ||
                        o.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase));
                    break;

                case "returns":
                    filteredOrders = allOrders.Where(o =>
                        o.Status.Equals("Returned", StringComparison.OrdinalIgnoreCase));
                    break;

                default:
                    filteredOrders = allOrders;
                    break;
            }

            return View(filteredOrders);
        }
    }
}