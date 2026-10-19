using DatabaseLayer.Data;
using DatabaseLayer.Interfaces;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseLayer.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly EcommerceDbContext _context;

        public CartItemRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task<CartItem?> GetByIdAsync(int cartItemId)
        {
            return await _context.CartItems.FindAsync(cartItemId);
        }

        public async Task<IEnumerable<CartItem>> GetByCartIdAsync(int cartId)
        {
            return await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .Include(ci => ci.Product)
                .ToListAsync();
        }

        public async Task UpdateAsync(CartItem cartItem)
        {
            _context.Entry(cartItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task RemoveAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int cartItemId)
        {
            return await _context.CartItems.AnyAsync(ci => ci.CartItemId == cartItemId);
        }

        public async Task<decimal> GetCartTotalAsync(int cartId)
        {
            return await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .SumAsync(ci => ci.Quantity * ci.UnitPrice);
        }
    }
}