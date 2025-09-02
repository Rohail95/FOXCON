using DatabaseLayer.Interfaces;
using DatabaseLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    
    public interface IDiscountService
    {
        Task<Discount?> GetDiscountByIdAsync(int id);
        Task<IEnumerable<Discount>> GetAllDiscountsAsync();
        Task CreateDiscountAsync(Discount discount);
        Task UpdateAsync(Discount discount, List<int>? productIds);
        Task DeleteDiscountAsync(int id);
        Task<IEnumerable<Discount>> GetActiveSiteWideDiscountsAsync();
    }

   
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task CreateDiscountAsync(Discount discount) => await _discountRepository.AddAsync(discount);
        public async Task<Discount?> GetDiscountByIdAsync(int id) => await _discountRepository.GetByIdAsync(id);
        public async Task<IEnumerable<Discount>> GetAllDiscountsAsync() => await _discountRepository.GetAllAsync();
        public async Task DeleteDiscountAsync(int id) => await _discountRepository.DeleteAsync(id);


        
        public async Task UpdateAsync(Discount discount, List<int>? productIds)
        {
            await _discountRepository.UpdateAsync(discount, productIds);
        }
        public async Task<IEnumerable<Discount>> GetActiveSiteWideDiscountsAsync()
        {
            return await _discountRepository.GetActiveSiteWideDiscountsAsync();
        }

    }
}