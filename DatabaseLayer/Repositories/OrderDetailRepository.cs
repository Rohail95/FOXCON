using DatabaseLayer.Data;
using DatabaseLayer.Interfaces;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseLayer.Repositories
{
    public class OrderDetailRepository(EcommerceDbContext context) : IOrderDetailRepository
    {
        private readonly EcommerceDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<OrderDetail?> GetByIdAsync(int orderDetailId)
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId);
        }

        public async Task<IEnumerable<OrderDetail>> GetAllAsync()
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .ToListAsync();
        }

        public async Task AddAsync(OrderDetail orderDetail)
        {
            if (orderDetail != null)
            {
                orderDetail.Subtotal = orderDetail.UnitPrice * orderDetail.Quantity;
                await _context.OrderDetails.AddAsync(orderDetail);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentNullException(nameof(orderDetail));
            }
        }

        public async Task UpdateAsync(OrderDetail orderDetail)
        {
            ArgumentNullException.ThrowIfNull(orderDetail);

            orderDetail.Subtotal = orderDetail.UnitPrice * orderDetail.Quantity;
            _context.OrderDetails.Update(orderDetail);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int orderDetailId)
        {
            var orderDetail = await GetByIdAsync(orderDetailId);
            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int orderDetailId)
        {
            return await _context.OrderDetails.AnyAsync(od => od.OrderDetailId == orderDetailId);
        }

        public async Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Include(od => od.Product)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDetail>> GetByProductIdAsync(int productId)
        {
            return await _context.OrderDetails
                .Where(od => od.ProductId == productId)
                .Include(od => od.Order)
                .ToListAsync();
        }

        public async Task<int> GetCountByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .CountAsync(od => od.OrderId == orderId);
        }

        public async Task<decimal> GetOrderTotalAsync(int orderId)
        {
            return await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .SumAsync(od => od.Subtotal);
        }

        public async Task UpdateQuantityAsync(int orderDetailId, int newQuantity)
        {
            var orderDetail = await GetByIdAsync(orderDetailId);
            if (orderDetail != null)
            {
                orderDetail.Quantity = newQuantity;
                orderDetail.Subtotal = orderDetail.UnitPrice * newQuantity;
                await UpdateAsync(orderDetail);
            }
        }

        public async Task<int> GetProductQuantityInOrderAsync(int orderId, int productId)
        {
            return await _context.OrderDetails
                .Where(od => od.OrderId == orderId && od.ProductId == productId)
                .SumAsync(od => od.Quantity);
        }

        public async Task UpdateUnitPriceAsync(int orderDetailId, decimal newPrice)
        {
            var orderDetail = await GetByIdAsync(orderDetailId);
            if (orderDetail != null)
            {
                orderDetail.UnitPrice = newPrice;
                orderDetail.Subtotal = newPrice * orderDetail.Quantity;
                await UpdateAsync(orderDetail);
            }
        }

        public async Task<decimal> GetProductSubtotalAsync(int orderDetailId)
        {
            var orderDetail = await GetByIdAsync(orderDetailId);
            return orderDetail?.Subtotal ?? 0;
        }

        public async Task AddRangeAsync(IEnumerable<OrderDetail> orderDetails)
        {
            if (orderDetails != null)
            {
                foreach (var od in orderDetails)
                {
                    od.Subtotal = od.UnitPrice * od.Quantity;
                }

                await _context.OrderDetails.AddRangeAsync(orderDetails);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentNullException(nameof(orderDetails));
            }
        }

        public async Task DeleteByOrderIdAsync(int orderId)
        {
            var items = await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            if (items.Count != 0)
            {
                _context.OrderDetails.RemoveRange(items);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateRangeAsync(IEnumerable<OrderDetail> orderDetails)
        {
            if (orderDetails != null)
            {
                foreach (var od in orderDetails)
                {
                    od.Subtotal = od.UnitPrice * od.Quantity;
                }

                _context.OrderDetails.UpdateRange(orderDetails);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentNullException(nameof(orderDetails));
            }
        }

        public async Task<Dictionary<int, int>> GetProductSalesCountAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .Where(od => od.Order.OrderDate >= startDate && od.Order.OrderDate <= endDate)
                .GroupBy(od => od.ProductId)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(od => od.Quantity));
        }

        public async Task<Dictionary<int, decimal>> GetProductRevenueAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .Where(od => od.Order.OrderDate >= startDate && od.Order.OrderDate <= endDate)
                .GroupBy(od => od.ProductId)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(od => od.Subtotal));
        }

        public async Task<int> GetTotalItemsSoldAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .Where(od => od.Order.OrderDate >= startDate && od.Order.OrderDate <= endDate)
                .SumAsync(od => od.Quantity);
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .Where(od => od.Order.OrderDate >= startDate && od.Order.OrderDate <= endDate)
                .SumAsync(od => od.Subtotal);
        }
    }
}