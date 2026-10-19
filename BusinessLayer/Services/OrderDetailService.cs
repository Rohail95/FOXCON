using DatabaseLayer.Interfaces;
using DatabaseLayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IOrderDetailService
    {
        Task<OrderDetail?> GetOrderDetailByIdAsync(int id);
        Task<IEnumerable<OrderDetail>> GetAllOrderDetailsAsync();
        Task CreateOrderDetailAsync(OrderDetail orderDetail);
        Task UpdateOrderDetailAsync(OrderDetail orderDetail);
        Task DeleteOrderDetailAsync(int id);
    }

    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IProductRepository _productRepository;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository, IProductRepository productRepository)
        {
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
        }

        public async Task CreateOrderDetailAsync(OrderDetail orderDetail)
        {
            var product = await _productRepository.GetByIdAsync(orderDetail.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException("Cannot create order detail: Product not found.");
            }
            orderDetail.UnitPrice = product.Price;
            await _orderDetailRepository.AddAsync(orderDetail);
        }

        public async Task DeleteOrderDetailAsync(int id)
        {
            await _orderDetailRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<OrderDetail>> GetAllOrderDetailsAsync()
        {
            return await _orderDetailRepository.GetAllAsync();
        }

        public async Task<OrderDetail?> GetOrderDetailByIdAsync(int id)
        {
            return await _orderDetailRepository.GetByIdAsync(id);
        }

        public async Task UpdateOrderDetailAsync(OrderDetail orderDetail)
        {
            await _orderDetailRepository.UpdateAsync(orderDetail);
        }
    }
}