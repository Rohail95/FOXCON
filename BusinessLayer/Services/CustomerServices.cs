using DatabaseLayer.Data;
using DatabaseLayer.Interfaces;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    // The definitive interface for the service
    public interface ICustomerService
    {
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task CreateCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int id);
        Task<Customer?> GetCustomerWithOrdersAsync(int id);
        Task<Customer?> GetCustomerByEmailAsync(string email);
        Task<Customer?> GetCustomerByUserIdAsync(string userId);
    }

    // The definitive implementation of the service
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly EcommerceDbContext _context; // Required for specific tracked queries

        public CustomerService(ICustomerRepository customerRepository, EcommerceDbContext context)
        {
            _customerRepository = customerRepository;
            _context = context; // This line fixes the "_context does not exist" error
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        public async Task<Customer?> GetCustomerWithOrdersAsync(int id)
        {
            // Using the DbContext directly here is best for complex queries
            return await _context.Customers
                .Include(c => c.Orders)
                .AsNoTracking() // Good for read-only pages
                .FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        public async Task CreateCustomerAsync(Customer customer)
        {
            await _customerRepository.AddAsync(customer);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            // The service simply passes the tracked entity to the repository to be saved
            await _customerRepository.UpdateAsync(customer);
        }

        public async Task DeleteCustomerAsync(int id)
        {
            await _customerRepository.DeleteAsync(id);
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await _customerRepository.GetByEmailAsync(email);
        }

        public async Task<Customer?> GetCustomerByUserIdAsync(string userId)
        {
            // THIS IS THE CRITICAL FIX: We use the DbContext to get a TRACKED entity.
            // We do NOT use AsNoTracking() here, because we intend to update this object.
            return await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);

        }
    }
}