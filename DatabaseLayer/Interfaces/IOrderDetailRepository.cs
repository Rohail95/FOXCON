using DatabaseLayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseLayer.Interfaces
{
    public interface IOrderDetailRepository
    {
       
        Task<OrderDetail?> GetByIdAsync(int orderDetailId);
        Task<IEnumerable<OrderDetail>> GetAllAsync();
        Task AddAsync(OrderDetail orderDetail);
        Task UpdateAsync(OrderDetail orderDetail);
        Task DeleteAsync(int orderDetailId);
        Task<bool> ExistsAsync(int orderDetailId);

       
        Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<OrderDetail>> GetByProductIdAsync(int productId);
        Task<int> GetCountByOrderIdAsync(int orderId);
        Task<decimal> GetOrderTotalAsync(int orderId);

        
        Task UpdateQuantityAsync(int orderDetailId, int newQuantity);
        Task<int> GetProductQuantityInOrderAsync(int orderId, int productId);

        
        Task UpdateUnitPriceAsync(int orderDetailId, decimal newPrice);
        Task<decimal> GetProductSubtotalAsync(int orderDetailId);

     
        Task AddRangeAsync(IEnumerable<OrderDetail> orderDetails);
        Task DeleteByOrderIdAsync(int orderId);
        Task UpdateRangeAsync(IEnumerable<OrderDetail> orderDetails);

       
        Task<Dictionary<int, int>> GetProductSalesCountAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, decimal>> GetProductRevenueAsync(DateTime startDate, DateTime endDate);
        Task<int> GetTotalItemsSoldAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate);
    }
}