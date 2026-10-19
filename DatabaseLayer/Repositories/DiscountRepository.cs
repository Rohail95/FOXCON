using DatabaseLayer.Data;
using DatabaseLayer.Interfaces;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseLayer.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly EcommerceDbContext _context;

        public DiscountRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        #region Core CRUD & Existence
        public async Task<Discount?> GetByIdAsync(int discountId)
        {
           
            return await _context.Discounts
                .Include(d => d.Products)
                .FirstOrDefaultAsync(d => d.DiscountId == discountId);
        }

        public async Task<IEnumerable<Discount>> GetAllAsync()
        {
            return await _context.Discounts
                .Include(d => d.Products)
                .OrderBy(d => d.StartDate)
                .ToListAsync();
        }

        public async Task AddAsync(Discount discount)
        {
            discount.IsActive ??= true;
            discount.CurrentUses ??= 0;
            await _context.Discounts.AddAsync(discount);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int discountId)
        {
            var discount = await _context.Discounts.FindAsync(discountId);
            if (discount != null)
            {
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Discount discountFromForm, List<int>? selectedProductIds)
        {
            var discountToUpdate = await _context.Discounts
                .Include(d => d.Products)
                .FirstOrDefaultAsync(d => d.DiscountId == discountFromForm.DiscountId);

            if (discountToUpdate == null) return;

            _context.Entry(discountToUpdate).CurrentValues.SetValues(discountFromForm);

            discountToUpdate.Products.Clear();
            if (selectedProductIds != null && selectedProductIds.Any())
            {
                var productsToLink = await _context.Products
                    .Where(p => selectedProductIds.Contains(p.ProductId)).ToListAsync();
                discountToUpdate.Products = productsToLink;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int discountId) => await _context.Discounts.AnyAsync(d => d.DiscountId == discountId);
        #endregion

        #region Business Logic & Queries
        public async Task<Discount?> GetByCodeAsync(string code)
        {
            return await _context.Discounts.AsNoTracking().FirstOrDefaultAsync(d => d.Code == code);
        }

        public async Task<IEnumerable<Discount>> GetActiveDiscountsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Discounts
                .Where(d => d.IsActive == true && d.StartDate <= now && d.EndDate >= now && (d.MaxUses == null || d.CurrentUses < d.MaxUses))
                .ToListAsync();
        }

        public async Task<IEnumerable<Discount>> GetUpcomingDiscountsAsync() => await _context.Discounts.Where(d => d.StartDate > DateTime.UtcNow).ToListAsync();

        public async Task<IEnumerable<Discount>> GetExpiredDiscountsAsync() => await _context.Discounts.Where(d => d.EndDate < DateTime.UtcNow).ToListAsync();

        public async Task<bool> IsDiscountValidAsync(string code, decimal orderAmount)
        {
            var now = DateTime.UtcNow;
            return await _context.Discounts
                .AnyAsync(d => d.Code == code && d.IsActive == true && d.StartDate <= now && d.EndDate >= now && (d.MinOrderAmount == null || orderAmount >= d.MinOrderAmount) && (d.MaxUses == null || d.CurrentUses < d.MaxUses));
        }

        public async Task<decimal> ApplyDiscountAsync(string code, decimal originalAmount)
        {
            var discount = await GetByCodeAsync(code);
            if (discount == null || !await IsDiscountValidAsync(code, originalAmount)) return originalAmount;

            return discount.DiscountType.ToLower() switch
            {
                "percentage" => originalAmount * (1 - (discount.DiscountValue / 100)),
                "fixed" or "fixed_amount" => Math.Max(0, originalAmount - discount.DiscountValue),
                _ => originalAmount
            };
        }

        public async Task IncrementUsageAsync(int discountId)
        {
            var discount = await _context.Discounts.FindAsync(discountId);
            if (discount != null) { discount.CurrentUses = (discount.CurrentUses ?? 0) + 1; await _context.SaveChangesAsync(); }
        }

        public async Task<IEnumerable<Discount>> GetDiscountsByDateRangeAsync(DateTime startDate, DateTime endDate) => await _context.Discounts.Where(d => d.StartDate <= endDate && d.EndDate >= startDate).ToListAsync();

        public async Task ActivateDiscountAsync(int discountId)
        {
            var discount = await _context.Discounts.FindAsync(discountId);
            if (discount != null) { discount.IsActive = true; await _context.SaveChangesAsync(); }
        }

        public async Task DeactivateDiscountAsync(int discountId)
        {
            var discount = await _context.Discounts.FindAsync(discountId);
            if (discount != null) { discount.IsActive = false; await _context.SaveChangesAsync(); }
        }
        #endregion

        #region Analytics
        public async Task<int> GetTotalUsesAsync(int discountId)
        {
            var uses = await _context.Discounts.Where(d => d.DiscountId == discountId).Select(d => d.CurrentUses).FirstOrDefaultAsync();
            return uses ?? 0;
        }

        public async Task<Dictionary<string, int>> GetDiscountTypeDistributionAsync()
        {
            return await _context.Discounts.GroupBy(d => d.DiscountType).ToDictionaryAsync(g => g.Key ?? "Unknown", g => g.Count());
        }

        public async Task<int> GetActiveDiscountCountAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Discounts.CountAsync(d => d.IsActive == true && d.StartDate <= now && d.EndDate >= now);
        }

        public async Task<IEnumerable<Discount>> GetActiveSiteWideDiscountsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Discounts
                .Where(d => d.IsActive == true &&
                            d.StartDate <= now &&
                            d.EndDate >= now &&
                            (d.MaxUses == null || d.CurrentUses < d.MaxUses) &&
                            !d.Products.Any())
                .ToListAsync();
        }

        #endregion
    }
}