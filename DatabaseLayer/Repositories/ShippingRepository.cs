using DatabaseLayer.Data;
using DatabaseLayer.Interfaces;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseLayer.Repositories
{
    public class ShippingRepository(EcommerceDbContext context) : IShippingRepository
    {
        private readonly EcommerceDbContext _context = context;

        public async Task<Shipping> GetByIdAsync(int shippingId)
        {
            var shipping = await _context.Shippings
                .Include(s => s.Order)
                .ThenInclude(o => o.Customer)
                .FirstOrDefaultAsync(s => s.ShippingId == shippingId) ?? throw new InvalidOperationException($"Shipping not found for ShippingId {shippingId}");
            return shipping;
        }

        public async Task<IEnumerable<Shipping>> GetAllAsync()
        {
            return await _context.Shippings
                .Include(s => s.Order)
                .OrderByDescending(s => s.Order.OrderDate)
                .ToListAsync();
        }

        public async Task AddAsync(Shipping shipping)
        {
            shipping.Status = "Processing"; 
            await _context.Shippings.AddAsync(shipping);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Shipping shipping)
        {
            _context.Shippings.Update(shipping);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int shippingId)
        {
            var shipping = await GetByIdAsync(shippingId);
            if (shipping != null)
            {
                _context.Shippings.Remove(shipping);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int shippingId)
        {
            return await _context.Shippings.AnyAsync(s => s.ShippingId == shippingId);
        }

        public async Task<Shipping> GetByOrderIdAsync(int orderId)
        {
            return await _context.Shippings
                .Include(s => s.Order)
                .FirstOrDefaultAsync(s => s.OrderId == orderId)
                ?? throw new InvalidOperationException($"Shipping not found for OrderId {orderId}");
        }

        public async Task<IEnumerable<Shipping>> GetByStatusAsync(string status)
        {
            return await _context.Shippings
                .Include(s => s.Order)
                .Where(s => s.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Shipping>> GetShippingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Shippings
                .Include(s => s.Order)
                .Where(s => s.Order.OrderDate >= startDate && s.Order.OrderDate <= endDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Shipping>> GetPendingShippingsAsync()
        {
            return await GetByStatusAsync("Processing");
        }

        public async Task<IEnumerable<Shipping>> GetDelayedShippingsAsync()
        {
            return await _context.Shippings
                .Include(s => s.Order)
                .Where(static s => s.Status == "Shipped" &&
                            s.EstimatedDelivery.HasValue &&
                            s.EstimatedDelivery.Value.ToDateTime(TimeOnly.MinValue) < DateTime.Now &&
                            s.ActualDelivery == null)
                .ToListAsync();
        }

        public async Task UpdateTrackingInfoAsync(int shippingId, string trackingNumber, string carrier)
        {
            var shipping = await GetByIdAsync(shippingId);
            if (shipping != null)
            {
                shipping.TrackingNumber = trackingNumber;
                shipping.Carrier = carrier;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateShippingStatusAsync(int shippingId, string status)
        {
            var shipping = await GetByIdAsync(shippingId);
            if (shipping != null)
            {
                shipping.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAsShippedAsync(int shippingId, DateTime estimatedDelivery)
        {
            var shipping = await GetByIdAsync(shippingId);
            if (shipping != null)
            {
                shipping.Status = "Shipped";
                shipping.EstimatedDelivery = DateOnly.FromDateTime(estimatedDelivery);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAsDeliveredAsync(int shippingId, DateTime actualDelivery)
        {
            var shipping = await GetByIdAsync(shippingId);
            if (shipping != null)
            {
                shipping.Status = "Delivered";
                shipping.ActualDelivery = DateOnly.FromDateTime(actualDelivery);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetTotalShippingCostAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Shippings
                .Where(s => s.Order.OrderDate >= startDate && s.Order.OrderDate <= endDate)
                .SumAsync(s => s.ShippingCost);
        }

        public async Task<Dictionary<string, int>> GetShippingStatusDistributionAsync()
        {
            return await _context.Shippings
                .GroupBy(s => s.Status)
                .ToDictionaryAsync(g => g.Key ?? "Unknown", g => g.Count());
        }

        public async Task<int> GetDeliveriesCountByCarrierAsync(string carrier)
        {
            return await _context.Shippings
                .Where(s => s.Carrier == carrier && s.Status == "Delivered")
                .CountAsync();
        }
    }
}