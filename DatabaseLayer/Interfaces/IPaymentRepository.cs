using DatabaseLayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseLayer.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int paymentId);
        Task<IEnumerable<Payment>> GetAllAsync();
        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task DeleteAsync(int paymentId);
        Task<bool> ExistsAsync(int paymentId);

       
        Task<IEnumerable<Payment>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<Payment>> GetByStatusAsync(string status);
        Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Payment>> GetByPaymentMethodAsync(string paymentMethod);
        Task<Payment?> GetByTransactionIdAsync(string transactionId);

      
        Task ProcessPaymentAsync(int paymentId);
        Task RefundPaymentAsync(int paymentId, decimal? amount = null);
        Task CapturePaymentAsync(int paymentId);
        Task VoidPaymentAsync(int paymentId);

        
        Task UpdatePaymentStatusAsync(int paymentId, string status);
        Task<string?> GetPaymentStatusAsync(int paymentId);

        
        Task UpdatePaymentAmountAsync(int paymentId, decimal newAmount);
        Task<decimal> GetPaymentAmountAsync(int paymentId);

        
        Task<decimal> GetTotalPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, decimal>> GetPaymentMethodDistributionAsync();
        Task<int> GetSuccessfulPaymentCountAsync();

        
        Task<int> GetTotalPaymentCountAsync();
        Task<DateTime?> GetLastPaymentDateAsync(int customerId);
    }
}