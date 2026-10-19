using DatabaseLayer.Data;
using DatabaseLayer.Interfaces;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseLayer.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly EcommerceDbContext _context;

        public ReviewRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Review review)
        {
            review.CreatedAt = System.DateTime.UtcNow;
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _context.Reviews.ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews.FindAsync(id);
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Entry(review).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public IQueryable<Review> GetAll()
        {
           
            return _context.Reviews.AsQueryable();
        }
    }
}