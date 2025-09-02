using DatabaseLayer.Models;
using System.Threading.Tasks;

namespace DatabaseLayer.Interfaces
{
   
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(string userId);
        Task<Cart> CreateAsync(Cart cart);
        Task AddItemAsync(CartItem item);
        Task UpdateItemAsync(CartItem item);
        Task RemoveItemAsync(CartItem item);
        Task<CartItem?> GetCartItemAsync(int cartId, int productId);
        Task ClearCartAsync(int cartId);
    }
}