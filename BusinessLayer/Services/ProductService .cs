using BusinessLayer.Services;
using DatabaseLayer.Interfaces;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IProductService
    {
        Task<Product?> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetAllProductsWithDiscountsAsync();
        Task CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsWithDiscountsAsync()
        {
            return await _productRepository.GetAll()
              .Include(p => p.Category)
              .Include(p => p.Discounts)
              .ToListAsync();
        }

       
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetAll()
                .Include(p => p.Category)
                .Include(p => p.Discounts)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.Customer)
                .FirstOrDefaultAsync(p => p.ProductId == id); ;
        }
        
        public async Task CreateProductAsync(Product product)
        {
            if (!product.CategoryId.HasValue)
            {
                throw new ArgumentException("Product must have a CategoryId.");
            }
           
            var category = await _categoryRepository.GetByIdAsync(product.CategoryId.Value);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID '{product.CategoryId.Value}' does not exist.");
            }
            await _productRepository.AddAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }
    }
}