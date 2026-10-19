using DatabaseLayer.Data;
using DatabaseLayer.Interfaces;
using DatabaseLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    // The definitive "contract" for the service
    public interface ICartService
    {
        Task<Cart?> GetCartByIdentifierAsync(string cartIdentifier);
        Task AddItemToCartAsync(string cartIdentifier, int productId, int quantity);
        Task RemoveItemFromCartAsync(string cartIdentifier, int cartItemId);
        Task UpdateItemQuantityAsync(string cartIdentifier, int cartItemId, int quantity);
        Task<bool> ApplyDiscountAsync(string cartIdentifier, string discountCode);
        Task ClearCartAsync(string cartIdentifier);
        Task AssignCartToUserAsync(string sessionId, string userId);
    }

    // The definitive, working implementation for BOTH guests and logged-in users
    public class CartService : ICartService
    {
        private readonly IDiscountRepository _discountRepository; // <-- NEEDED
        private readonly EcommerceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartService(
            IDiscountRepository discountRepository, // <-- ADDED TO CONSTRUCTOR
            EcommerceDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _discountRepository = discountRepository; // <-- INITIALIZED
            _context = context;
            _userManager = userManager;
        }

        public async Task<Cart?> GetCartByIdentifierAsync(string cartIdentifier)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == cartIdentifier || c.SessionId == cartIdentifier);
        }

        public async Task AddItemToCartAsync(string cartIdentifier, int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.StockQuantity < quantity) return;

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == cartIdentifier || c.SessionId == cartIdentifier);

            if (cart == null)
            {
                cart = new Cart();
                bool isUser = await _context.Users.AnyAsync(u => u.Id == cartIdentifier);

                if (isUser) { cart.UserId = cartIdentifier; }
                else { cart.SessionId = cartIdentifier; }

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
            {
                _context.CartItems.Add(new CartItem { CartId = cart.CartId, ProductId = productId, Quantity = quantity });
            }
            else
            {
                cartItem.Quantity += quantity;
            }
            await _context.SaveChangesAsync();
        }

        public async Task AssignCartToUserAsync(string sessionId, string userId)
        {
            // 1. Find the guest cart using the session ID
            var guestCart = await GetCartByIdentifierAsync(sessionId);
            if (guestCart != null && guestCart.CartItems.Any())
            {
                // 2. Get the user's permanent cart
                var userCart = await GetCartByIdentifierAsync(userId);
                if (userCart == null)
                {
                    // If the user has no cart, just assign the guest cart to them
                    guestCart.SessionId = null;
                    guestCart.UserId = userId;
                }
                else
                {
                    // If the user already has a cart, merge the items
                    foreach (var guestItem in guestCart.CartItems)
                    {
                        var userItem = userCart.CartItems.FirstOrDefault(ci => ci.ProductId == guestItem.ProductId);
                        if (userItem != null)
                        {
                            // If item exists, add quantity
                            userItem.Quantity += guestItem.Quantity;
                        }
                        else
                        {
                            // If item is new, move it to the user's cart
                            guestItem.CartId = userCart.CartId;
                        }
                    }
                    // Delete the now-empty guest cart
                    _context.Carts.Remove(guestCart);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveItemFromCartAsync(string cartIdentifier, int cartItemId)
        {
            var cartItem = await _context.CartItems.Include(ci => ci.Cart).FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);
            if (cartItem != null && (cartItem.Cart.UserId == cartIdentifier || cartItem.Cart.SessionId == cartIdentifier))
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateItemQuantityAsync(string cartIdentifier, int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.Include(ci => ci.Cart).FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);
            if (cartItem != null && (cartItem.Cart.UserId == cartIdentifier || cartItem.Cart.SessionId == cartIdentifier))
            {
                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ApplyDiscountAsync(string cartIdentifier, string discountCode)
        {
            var cart = await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(c => c.UserId == cartIdentifier || c.SessionId == cartIdentifier);
            if (cart == null) return false;

            var originalTotal = cart.CartItems.Sum(i => (i.Product?.Price ?? 0) * i.Quantity);
            var discountedTotal = await _discountRepository.ApplyDiscountAsync(discountCode, originalTotal);
            var discountAmount = originalTotal - discountedTotal;

            if (discountAmount >= 0) // Allows for free items
            {
                cart.AppliedDiscountCode = discountCode;
                cart.DiscountAmount = discountAmount;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task ClearCartAsync(string cartIdentifier)
        {
            var cart = await GetCartByIdentifierAsync(cartIdentifier);
            if (cart != null && cart.CartItems.Any())
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                // Important: Also clear any applied discount on the cart itself
                cart.AppliedDiscountCode = null;
                cart.DiscountAmount = 0;
                await _context.SaveChangesAsync();
            }
        }
    }
}