using DatabaseLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseLayer.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        IQueryable<Order> GetAll();
    }
}