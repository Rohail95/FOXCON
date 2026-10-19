using DatabaseLayer.Interfaces;
using DatabaseLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IShippingService
    {
        Task<Shipping?> GetShippingByIdAsync(int id);
        Task<IEnumerable<Shipping>> GetAllShippingsAsync();
        Task CreateShippingAsync(Shipping shipping);
        Task UpdateShippingAsync(Shipping shipping);
        Task DeleteShippingAsync(int id);
    }

    public class ShippingService : IShippingService
    {
        private readonly IShippingRepository _shippingRepository;

        public ShippingService(IShippingRepository shippingRepository)
        {
            _shippingRepository = shippingRepository;
        }

        public async Task CreateShippingAsync(Shipping shipping)
        {
            await _shippingRepository.AddAsync(shipping);
        }

        public async Task DeleteShippingAsync(int id)
        {
            await _shippingRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Shipping>> GetAllShippingsAsync()
        {
            return await _shippingRepository.GetAllAsync();
        }



        public async Task<Shipping?> GetShippingByIdAsync(int id)
        {
            return await _shippingRepository.GetByIdAsync(id);
        }

        public async Task UpdateShippingAsync(Shipping shipping)
        {
            await _shippingRepository.UpdateAsync(shipping);
        }
    }
}