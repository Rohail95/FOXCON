using DatabaseLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
namespace DatabaseLayer.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int id);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        IQueryable<Customer> GetAll();
        Task<Customer?> GetCustomerWithOrdersAsync(int id);
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer?> GetByUserIdAsync(string userId);
    }
}