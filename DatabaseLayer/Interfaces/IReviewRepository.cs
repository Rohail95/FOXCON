using DatabaseLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseLayer.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review?> GetByIdAsync(int id);
        Task<IEnumerable<Review>> GetAllAsync();
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(int id);
        IQueryable<Review> GetAll();
    }
}