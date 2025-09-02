using BusinessLayer.Services;
using DatabaseLayer.Models;
using EcommerceWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWeb.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(
            ICartService cartService,
            IOrderService orderService,
            ICustomerService customerService,
            UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _orderService = orderService;
            _customerService = customerService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.Id == null)
            {
                return Challenge();
            }

            var cart = await _cartService.GetCartAsync(user.Id);

            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var viewModel = new CheckoutViewModel
            {
                CartItems = cart.CartItems,
                CartTotal = cart.CartItems.Sum(item => item.Product?.Price * item.Quantity ?? 0),
                Email = user.Email ?? string.Empty
            };

           
            return View("~/Views/Order/Checkout.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CheckoutViewModel viewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.Email == null || user.Id == null)
            {
                return Challenge();
            }

            var cart = await _cartService.GetCartAsync(user.Id);

            if (cart == null || !cart.CartItems.Any())
            {
                ModelState.AddModelError("", "Your cart is empty. Please add items before checking out.");
            }

            var customer = await _customerService.GetCustomerByEmailAsync(user.Email);
            if (customer == null)
            {
                ModelState.AddModelError("", "Could not find a valid customer account. Please contact support.");
            }

            if (ModelState.IsValid)
            {
                var order = new Order
                {
                    CustomerId = customer!.CustomerId,
                    ShippingAddress = $"{viewModel.Address}, {viewModel.City}, {viewModel.PostalCode}, {viewModel.Country}",
                    BillingAddress = $"{viewModel.Address}, {viewModel.City}, {viewModel.PostalCode}, {viewModel.Country}",
                    PaymentMethod = viewModel.PaymentMethod,
                    OrderDate = DateTime.UtcNow,
                    Status = "Pending",
                    OrderDetails = cart!.CartItems.Select(item => new OrderDetail
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price
                    }).ToList()
                };

                await _orderService.CreateOrderAsync(order);
                await _cartService.ClearCartAsync(user.Id);

                return RedirectToAction("OrderComplete", new { orderId = order.OrderId });
            }

            if (cart != null)
            {
                viewModel.CartItems = cart.CartItems;
                viewModel.CartTotal = cart.CartItems.Sum(item => item.Product?.Price * item.Quantity ?? 0);
            }

            return View("~/Views/Order/Checkout.cshtml", viewModel);
        }

        public IActionResult OrderComplete(int orderId)
        {
            ViewBag.OrderId = orderId;
            
            return View("~/Views/Order/OrderComplete.cshtml");
        }
    }
}