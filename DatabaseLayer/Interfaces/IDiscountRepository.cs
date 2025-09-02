using DatabaseLayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseLayer.Interfaces
{
    public interface IDiscountRepository
    {
       
        Task<Discount?> GetByIdAsync(int discountId);
        Task<IEnumerable<Discount>> GetAllAsync();
        Task AddAsync(Discount discount);
        Task UpdateAsync(Discount discount, List<int>? selectedProductIds);
        Task DeleteAsync(int discountId);
        Task<bool> ExistsAsync(int discountId);
        Task<Discount?> GetByCodeAsync(string code);
        Task<IEnumerable<Discount>> GetActiveDiscountsAsync();
        Task<IEnumerable<Discount>> GetUpcomingDiscountsAsync();
        Task<IEnumerable<Discount>> GetExpiredDiscountsAsync();
        Task<IEnumerable<Discount>> GetDiscountsByDateRangeAsync(DateTime startDate, DateTime endDate);

        
        Task<bool> IsDiscountValidAsync(string code, decimal orderAmount);
        Task<decimal> ApplyDiscountAsync(string code, decimal originalAmount);
        Task IncrementUsageAsync(int discountId);

        Task ActivateDiscountAsync(int discountId);
        Task DeactivateDiscountAsync(int discountId);

       
        Task<int> GetTotalUsesAsync(int discountId);
        Task<Dictionary<string, int>> GetDiscountTypeDistributionAsync();
        Task<int> GetActiveDiscountCountAsync();
        Task<IEnumerable<Discount>> GetActiveSiteWideDiscountsAsync();
    }
}