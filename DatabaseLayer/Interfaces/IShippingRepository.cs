using DatabaseLayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseLayer.Interfaces
{
    public interface IShippingRepository
    {
        Task<Shipping> GetByIdAsync(int shippingId);
        Task<IEnumerable<Shipping>> GetAllAsync();
        Task AddAsync(Shipping shipping);
        Task UpdateAsync(Shipping shipping);
        Task DeleteAsync(int shippingId);
        Task<bool> ExistsAsync(int shippingId);

        Task<Shipping> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<Shipping>> GetByStatusAsync(string status);
        Task<IEnumerable<Shipping>> GetShippingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Shipping>> GetPendingShippingsAsync();
        Task<IEnumerable<Shipping>> GetDelayedShippingsAsync();

        Task UpdateTrackingInfoAsync(int shippingId, string trackingNumber, string carrier);
        Task UpdateShippingStatusAsync(int shippingId, string status);

        Task MarkAsShippedAsync(int shippingId, DateTime estimatedDelivery);
        Task MarkAsDeliveredAsync(int shippingId, DateTime actualDelivery);

        Task<decimal> GetTotalShippingCostAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, int>> GetShippingStatusDistributionAsync();
        Task<int> GetDeliveriesCountByCarrierAsync(string carrier);
    }
}