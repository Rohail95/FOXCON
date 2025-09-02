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
    public class OrderRepository : IOrderRepository
    {
        private readonly EcommerceDbContext _context;

        public OrderRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public IQueryable<Order> GetAll()
        {
            return _context.Orders.AsQueryable();
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }


        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Payments)
                .Include(o => o.Shippings)
                .FirstOrDefaultAsync(o => o.OrderId == orderId)
                ?? throw new InvalidOperationException($"Order with ID {orderId} not found.");
        }

        public async Task AddAsync(Order order)
        {
            order.OrderDate = DateTime.UtcNow;
            order.Status = "Pending";
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int orderId)
        {
            var order = await GetByIdAsync(orderId);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<bool> ExistsAsync(int orderId)
        {
            return await _context.Orders.AnyAsync(o => o.OrderId == orderId);
        }

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(string status)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .Include(o => o.Customer)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Include(o => o.Customer)
                .ToListAsync();
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await GetByIdAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task CancelOrderAsync(int orderId)
        {
            var order = await GetByIdAsync(orderId);
            if (order != null && order.Status != "Shipped" && order.Status != "Delivered")
            {
                order.Status = "Cancelled";
                await _context.SaveChangesAsync();
            }
        }

        public async Task FulfillOrderAsync(int orderId)
        {
            var order = await GetByIdAsync(orderId);
            if (order != null && order.Status == "Paid")
            {
                order.Status = "Fulfilled";
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetOrderTotalWithShippingAsync(int orderId)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null) return 0;

            var shippingCost = await _context.Shippings
                .Where(s => s.OrderId == orderId)
                .SumAsync(s => s.ShippingCost);

            return order.TotalAmount + shippingCost;
        }

        public async Task<bool> IsOrderPaidAsync(int orderId)
        {
            var totalPaid = await _context.Payments
                .Where(p => p.OrderId == orderId && p.Status == "Completed")
                .SumAsync(p => p.Amount);

            var orderTotal = await GetOrderTotalWithShippingAsync(orderId);

            return totalPaid >= orderTotal;
        }

        public async Task<bool> HasShippingAsync(int orderId)
        {
            return await _context.Shippings
                .AnyAsync(s => s.OrderId == orderId);
        }

        public async Task<Shipping> GetOrderShippingAsync(int orderId)
        {
            var shipping = await _context.Shippings
                .FirstOrDefaultAsync(s => s.OrderId == orderId);

            if (shipping == null)
                throw new InvalidOperationException($"Shipping for Order ID {orderId} not found.");

            return shipping;
        }

        public async Task<decimal> GetRevenueByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status != "Cancelled")
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<Dictionary<string, int>> GetOrderStatusDistributionAsync()
        {
            return await _context.Orders
                .GroupBy(o => o.Status)
                .ToDictionaryAsync(g => g.Key ?? "Unknown", g => g.Count());
        }

        public async Task<int> GetCustomerOrderCountAsync(int customerId)
        {
            return await _context.Orders
                .CountAsync(o => o.CustomerId == customerId);
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsAsync(int orderId)
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
        }


        public async Task AddOrderDetailAsync(OrderDetail orderDetail)
        {
            await _context.OrderDetails.AddAsync(orderDetail);
            await _context.SaveChangesAsync();
        }

        public Task RecordDiscountUsageAsync(int value, int discountId, decimal v)
        {
            throw new NotImplementedException();
        }

    }
}