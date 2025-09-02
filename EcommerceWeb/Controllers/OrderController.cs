using BusinessLayer.Services;
using DatabaseLayer.Interfaces;
using DatabaseLayer.Models;
using EcommerceWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWeb.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICustomerRepository _customerRepository;

        public OrderController(IOrderService o, ICartService c, UserManager<ApplicationUser> u, ICustomerRepository cr)
        {
            _orderService = o; _cartService = c; _userManager = u; _customerRepository = cr;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var cart = await _cartService.GetCartAsync(user.Id);
            if (cart == null || !cart.CartItems.Any()) return RedirectToAction("Index", "Cart");

            var customer = (await _customerRepository.GetAllAsync()).FirstOrDefault(c => c.UserId == user.Id);

            var viewModel = new CheckoutViewModel
            {
                CartItems = cart.CartItems,
                CartTotal = cart.CartItems.Sum(i => i.Quantity * i.UnitPrice),
                Email = user.Email ?? string.Empty,
                FirstName = customer?.FirstName ?? user.FirstName ?? string.Empty,
                LastName = customer?.LastName ?? user.LastName ?? string.Empty,
                Address = customer?.Address ?? string.Empty,
                City = customer?.City ?? string.Empty,
                PostalCode = customer?.PostalCode ?? string.Empty,
                Country = customer?.Country ?? string.Empty,
                Phone = customer?.Phone ?? string.Empty
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel viewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var cart = await _cartService.GetCartAsync(user.Id);

            if (cart == null || !cart.CartItems.Any())
            {
                ModelState.AddModelError("", "Your cart is empty, please add items before checking out.");
            }

            if (!ModelState.IsValid)
            {
                if (cart != null && cart.CartItems.Any())
                {
                    viewModel.CartItems = cart.CartItems;
                    viewModel.CartTotal = cart.CartItems.Sum(i => i.Quantity * i.UnitPrice);
                }
                else
                {
                    viewModel.CartItems = new List<CartItem>();
                    viewModel.CartTotal = 0;
                }
                return View(viewModel);
            }

            var customer = (await _customerRepository.GetAllAsync()).FirstOrDefault(c => c.UserId == user.Id);
            if (customer == null)
            {
                customer = new Customer { UserId = user.Id, User = user, FirstName = viewModel.FirstName, LastName = viewModel.LastName, Email = viewModel.Email };
                await _customerRepository.AddAsync(customer);
            }

            customer.Address = viewModel.Address;
            customer.City = viewModel.City;
            customer.PostalCode = viewModel.PostalCode;
            customer.Country = viewModel.Country;
            customer.Phone = viewModel.Phone;
            await _customerRepository.UpdateAsync(customer);

            var newOrder = new Order
            {
                CustomerId = customer.CustomerId,
                TotalAmount = viewModel.CartTotal,
                ShippingAddress = $"{viewModel.FirstName} {viewModel.LastName}, {viewModel.Address}, {viewModel.City}, {viewModel.PostalCode}, {viewModel.Country}",
                BillingAddress = $"{viewModel.FirstName} {viewModel.LastName}, {viewModel.Address}, {viewModel.City}, {viewModel.PostalCode}, {viewModel.Country}",
                PaymentMethod = viewModel.PaymentMethod,
                OrderDetails = cart!.CartItems.Select(ci => new OrderDetail
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice
                }).ToList()
            };

            await _orderService.CreateOrderAsync(newOrder);

            await _cartService.ClearCartAsync(user.Id);

            return RedirectToAction("OrderComplete");
        }

        public IActionResult OrderComplete()
        {
            return View();
        }
    }
}