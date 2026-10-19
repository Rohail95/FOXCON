using DatabaseLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseLayer.Interfaces
{
    public interface ICartItemRepository
    {
        Task<CartItem?> GetByIdAsync(int cartItemId); 
        Task<IEnumerable<CartItem>> GetByCartIdAsync(int cartId);
        Task AddAsync(CartItem cartItem);
        Task UpdateAsync(CartItem cartItem);
        Task RemoveAsync(int cartItemId); 
        Task<bool> ExistsAsync(int cartItemId);
        Task<decimal> GetCartTotalAsync(int cartId); 
    }
}