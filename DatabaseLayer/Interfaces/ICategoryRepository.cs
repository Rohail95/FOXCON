using DatabaseLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseLayer.Interfaces
{
  
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
    }
}