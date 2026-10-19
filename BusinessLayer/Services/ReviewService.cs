using DatabaseLayer.Interfaces;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    // The Interface with the required methods
    public interface IReviewService
    {
        Task<Review?> GetReviewByIdAsync(int id);
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task<IEnumerable<Review>> GetAllReviewsWithDetailsAsync();
        Task CreateReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(int id);
    }

    // The single, correct implementation
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;

        public ReviewService(
            IReviewRepository reviewRepository,
            IProductRepository productRepository,
            ICustomerRepository customerRepository)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await _reviewRepository.GetAllAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await _reviewRepository.GetByIdAsync(id);
        }

        // This is the one and only implementation. It now works because the repository is fixed.
        public async Task<IEnumerable<Review>> GetAllReviewsWithDetailsAsync()
        {
            return await _reviewRepository.GetAll()
                .Include(r => r.Product)
                .Include(r => r.Customer)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        public async Task CreateReviewAsync(Review review)
        {
            if (!await _productRepository.ExistsAsync(review.ProductId))
            {
                throw new Exception("Cannot create review: Product does not exist.");
            }
            if (!await _customerRepository.ExistsAsync(review.CustomerId))
            {
                throw new Exception("Cannot create review: Customer does not exist.");
            }

            await _reviewRepository.AddAsync(review);
        }

        public async Task UpdateReviewAsync(Review review)
        {
            await _reviewRepository.UpdateAsync(review);
        }

        public async Task DeleteReviewAsync(int id)
        {
            await _reviewRepository.DeleteAsync(id);
        }
    }
}