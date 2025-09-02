using BusinessLayer.Services;
using DatabaseLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceWeb.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var cart = await _cartService.GetCartAsync(user.Id);

            
            if (cart == null)
            {
                
                cart = new Cart { UserId = user.Id, User = user, CartItems = new List<CartItem>() };
            }

            return View(cart);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            await _cartService.AddItemToCartAsync(user.Id, productId, quantity);

            TempData["SuccessMessage"] = "Item added to cart successfully!";

            return RedirectToAction("Index", "Products");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId) 
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            await _cartService.RemoveItemFromCartAsync(user.Id, cartItemId);

            return RedirectToAction(nameof(Index));
        }
    }
}