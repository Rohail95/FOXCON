using DatabaseLayer.Interfaces;
using DatabaseLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
   
    public interface ICartItemService
    {
        Task<CartItem?> GetCartItemByIdAsync(int id);
        Task CreateCartItemAsync(CartItem cartItem);
        Task UpdateCartItemAsync(CartItem cartItem);
        Task DeleteCartItemAsync(int id);
    }

   
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;

        public CartItemService(ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }

        public async Task CreateCartItemAsync(CartItem cartItem)
        {
            await _cartItemRepository.AddAsync(cartItem);
        }

        public async Task DeleteCartItemAsync(int id)
        {
            await _cartItemRepository.RemoveAsync(id);
        }

        public async Task<CartItem?> GetCartItemByIdAsync(int id)
        {
            return await _cartItemRepository.GetByIdAsync(id);
        }

        public async Task UpdateCartItemAsync(CartItem cartItem)
        {
            await _cartItemRepository.UpdateAsync(cartItem);
        }
    }
}