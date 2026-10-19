using BusinessLayer.Services;
using DatabaseLayer.Models;
using EcommerceWeb.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWeb.Controllers
{
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

        private string GetCartIdentifier()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return _userManager.GetUserId(User);
            }

            string sessionId = HttpContext.Session.GetString("CartSessionId");
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CartSessionId", sessionId);
            }
            return sessionId;
        }

        [HttpPost]
        public async Task<IActionResult> BuyNow(int productId)
        {
            string cartIdentifier = GetCartIdentifier();
            await _cartService.ClearCartAsync(cartIdentifier);
            await _cartService.AddItemToCartAsync(cartIdentifier, productId, 1);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string cartIdentifier = GetCartIdentifier();
            var cart = await _cartService.GetCartByIdentifierAsync(cartIdentifier);

            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Products");
            }

            var viewModel = new CheckoutViewModel
            {
                CartItems = cart.CartItems,
                CartTotal = cart.CartItems.Sum(item => (item.Product?.Price ?? 0) * item.Quantity)
            };

            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var customer = await _customerService.GetCustomerByUserIdAsync(user.Id);
                    if (customer != null)
                    {
                        viewModel.Email = customer.Email ?? user.Email;
                        viewModel.FirstName = customer.FirstName ?? user.FirstName;
                        viewModel.LastName = customer.LastName ?? user.LastName;
                        viewModel.Address = customer.Address;
                        viewModel.City = customer.City;
                        viewModel.PostalCode = customer.PostalCode;
                        viewModel.Country = customer.Country;
                        viewModel.Phone = customer.Phone;
                    }
                }
            }

            return View("~/Views/Order/Checkout.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutViewModel viewModel)
        {
            string cartIdentifier = GetCartIdentifier();
            var cart = await _cartService.GetCartByIdentifierAsync(cartIdentifier);

            if (cart == null || !cart.CartItems.Any())
            {
                ModelState.AddModelError("", "Your shopping cart is empty.");
            }
            else
            {
                viewModel.CartItems = cart.CartItems;
                viewModel.CartTotal = cart.CartItems.Sum(i => (i.Product?.Price ?? 0) * i.Quantity);
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Order/Checkout.cshtml", viewModel);
            }

            Customer customer;
            ApplicationUser user = null;

            if (User.Identity?.IsAuthenticated == true)
            {
                user = await _userManager.GetUserAsync(User);
                customer = await _customerService.GetCustomerByUserIdAsync(user!.Id);

                if (customer == null)
                {
                    ModelState.AddModelError("", "Could not find a valid customer account. Please contact support.");
                    return View("~/Views/Order/Checkout.cshtml", viewModel);
                }

                customer.FirstName = viewModel.FirstName;
                customer.LastName = viewModel.LastName;
                customer.Address = viewModel.Address;
                customer.City = viewModel.City;
                customer.PostalCode = viewModel.PostalCode;
                customer.Country = viewModel.Country;
                customer.Phone = viewModel.Phone;
                await _customerService.UpdateCustomerAsync(customer);
            }
            else
            {
                customer = new Customer
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Email = viewModel.Email,
                    Address = viewModel.Address,
                    City = viewModel.City,
                    PostalCode = viewModel.PostalCode,
                    Country = viewModel.Country,
                    Phone = viewModel.Phone
                };
                await _customerService.CreateCustomerAsync(customer);
            }

            var order = new Order
            {
                CustomerId = customer.CustomerId,
                UserId = user?.Id,
                User = user,
                ShippingAddress = $"{viewModel.Address}, {viewModel.City}, {viewModel.PostalCode}, {viewModel.Country}",
                BillingAddress = $"{viewModel.Address}, {viewModel.City}, {viewModel.PostalCode}, {viewModel.Country}",
                PaymentMethod = viewModel.PaymentMethod,
                Status = "Pending",
                OrderDate = DateTime.UtcNow,
                OrderDetails = cart!.CartItems.Select(item => new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                }).ToList()
            };

            await _orderService.CreateOrderAsync(order);
            await _cartService.ClearCartAsync(cartIdentifier);

            if (User.Identity?.IsAuthenticated == false)
            {
                HttpContext.Session.Remove("CartSessionId");
            }

            return RedirectToAction("OrderComplete", new { orderId = order.OrderId });
        }

        [HttpGet]
        public IActionResult OrderComplete(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View("~/Views/Order/OrderComplete.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> AddToCartAndCheckout(int productId)
        // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        {
            string cartIdentifier = GetCartIdentifier();
            // This logic is now ONLY for the "Buy Now" button
            await _cartService.ClearCartAsync(cartIdentifier);
            await _cartService.AddItemToCartAsync(cartIdentifier, productId, 1);

            return RedirectToAction(nameof(Index));
        }
    }
}