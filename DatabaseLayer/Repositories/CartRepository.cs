using DatabaseLayer.Data;
using DatabaseLayer.Interfaces;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseLayer.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly EcommerceDbContext _context;
        public CartRepository(EcommerceDbContext context) { _context = context; }

        public async Task<Cart?> GetByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart> CreateAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task AddItemAsync(CartItem item)
        {
            _context.CartItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(CartItem item)
        {
            _context.CartItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(CartItem item)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        public async Task ClearCartAsync(int cartId)
        {
            var itemsToRemove = _context.CartItems.Where(ci => ci.CartId == cartId);

            if (itemsToRemove.Any())
            {
                _context.CartItems.RemoveRange(itemsToRemove);
                await _context.SaveChangesAsync();
            }
        }
    }
}