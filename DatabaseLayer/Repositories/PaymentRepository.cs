using DatabaseLayer.Data;
using DatabaseLayer.Interfaces;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseLayer.Repositories
{
    public class PaymentRepository(EcommerceDbContext context) : IPaymentRepository
    {
        private readonly EcommerceDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<Payment?> GetByIdAsync(int paymentId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Order)
                .ToListAsync();
        }

        public async Task AddAsync(Payment payment)
        {
            ArgumentNullException.ThrowIfNull(payment);

            payment.PaymentDate = DateTime.UtcNow;
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Payment payment)
        {
            ArgumentNullException.ThrowIfNull(payment);

            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int paymentId)
        {
            var payment = await GetByIdAsync(paymentId);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int paymentId)
        {
            return await _context.Payments.AnyAsync(p => p.PaymentId == paymentId);
        }

        public async Task<IEnumerable<Payment>> GetByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .Where(p => p.OrderId == orderId)
                .Include(p => p.Order)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Status cannot be null or empty", nameof(status));
            }

            return await _context.Payments
                .Where(p => p.Status == status)
                .Include(p => p.Order)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be before end date", nameof(startDate));
            }

            return await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .Include(p => p.Order)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByPaymentMethodAsync(string paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                throw new ArgumentException("Payment method cannot be null or empty", nameof(paymentMethod));
            }

            return await _context.Payments
                .Where(p => p.PaymentMethod == paymentMethod)
                .Include(p => p.Order)
                .ToListAsync();
        }

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
            {
                throw new ArgumentException("Transaction ID cannot be null or empty", nameof(transactionId));
            }

            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task ProcessPaymentAsync(int paymentId)
        {
            var payment = await GetByIdAsync(paymentId) ?? throw new KeyNotFoundException($"Payment with ID {paymentId} not found");

            payment.Status = "Processed";
            payment.PaymentDate = DateTime.UtcNow;
            await UpdateAsync(payment);
        }

        public async Task RefundPaymentAsync(int paymentId, decimal? amount = null)
        {
            var payment = await GetByIdAsync(paymentId) ?? throw new KeyNotFoundException($"Payment with ID {paymentId} not found");

            payment.Status = amount == null ? "FullyRefunded" : "PartiallyRefunded";
            await UpdateAsync(payment);
        }

        public async Task CapturePaymentAsync(int paymentId)
        {
            var payment = await GetByIdAsync(paymentId) ?? throw new KeyNotFoundException($"Payment with ID {paymentId} not found");

            payment.Status = "Captured";
            await UpdateAsync(payment);
        }

        public async Task VoidPaymentAsync(int paymentId)
        {
            var payment = await GetByIdAsync(paymentId) ?? throw new KeyNotFoundException($"Payment with ID {paymentId} not found");

            payment.Status = "Voided";
            await UpdateAsync(payment);
        }

        public async Task UpdatePaymentStatusAsync(int paymentId, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Status cannot be null or empty", nameof(status));
            }

            var payment = await GetByIdAsync(paymentId) ?? throw new KeyNotFoundException($"Payment with ID {paymentId} not found");

            payment.Status = status;
            await UpdateAsync(payment);
        }

        public async Task<string?> GetPaymentStatusAsync(int paymentId)
        {
            var payment = await GetByIdAsync(paymentId);
            return payment?.Status;
        }


        public async Task UpdatePaymentAmountAsync(int paymentId, decimal newAmount)
        {
            var payment = await GetByIdAsync(paymentId) ?? throw new KeyNotFoundException($"Payment with ID {paymentId} not found");

            payment.Amount = newAmount;
            await UpdateAsync(payment);
        }

        public async Task<decimal> GetPaymentAmountAsync(int paymentId)
        {
            var payment = await GetByIdAsync(paymentId) ?? throw new KeyNotFoundException($"Payment with ID {paymentId} not found");
            return payment.Amount;
        }

        public async Task<decimal> GetTotalPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be before end date", nameof(startDate));
            }

            return await _context.Payments
                .Where(p => p.PaymentDate >= startDate &&
                           p.PaymentDate <= endDate &&
                           p.Status == "Completed")
                .SumAsync(p => p.Amount);
        }

        public async Task<Dictionary<string, decimal>> GetPaymentMethodDistributionAsync()
        {
            return await _context.Payments
                .GroupBy(p => p.PaymentMethod)
                .Select(g => new {
                    Method = g.Key ?? "Unknown",
                    Total = g.Sum(p => p.Amount)
                })
                .ToDictionaryAsync(x => x.Method, x => x.Total);
        }

        public async Task<int> GetSuccessfulPaymentCountAsync()
        {
            return await _context.Payments
                .CountAsync(p => p.Status == "Completed");
        }

        public async Task<int> GetTotalPaymentCountAsync()
        {
            return await _context.Payments.CountAsync();
        }

        public async Task<DateTime?> GetLastPaymentDateAsync(int customerId)
        {
            return await _context.Payments
                .Where(p => p.Order != null && p.Order.CustomerId == customerId)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => p.PaymentDate)
                .FirstOrDefaultAsync();
        }
    }
}