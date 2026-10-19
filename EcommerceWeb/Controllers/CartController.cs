using BusinessLayer.Services;
using DatabaseLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceWeb.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
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

        public async Task<IActionResult> Index()
        {
            string cartIdentifier = GetCartIdentifier();
            var cart = await _cartService.GetCartByIdentifierAsync(cartIdentifier);

            if (cart == null)
            {
                cart = new Cart { CartItems = new List<CartItem>() };
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            string cartIdentifier = GetCartIdentifier();
            await _cartService.AddItemToCartAsync(cartIdentifier, productId, quantity);
            TempData["SuccessMessage"] = "Item added to cart successfully!";
            return RedirectToAction("Index", "Products");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            string cartIdentifier = GetCartIdentifier();
            await _cartService.RemoveItemFromCartAsync(cartIdentifier, cartItemId);
            TempData["SuccessMessage"] = "Item removed from cart.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            string cartIdentifier = GetCartIdentifier();
            if (quantity < 1) { quantity = 1; }
            await _cartService.UpdateItemQuantityAsync(cartIdentifier, cartItemId, quantity);
            return RedirectToAction(nameof(Index));
        }
    }
}