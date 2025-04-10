using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletManager.Blazor.Models;

namespace WalletManager.Blazor.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<List<TransactionDto>> GetAllTransactionsAsync();
        Task<List<TransactionDto>> GetTransactionsByWalletIdAsync(int walletId);
        Task<TransactionDto?> GetTransactionByIdAsync(int id);
        Task<TransactionDto?> CreateTransactionAsync(CreateTransactionDto transaction);
        Task<TransactionDto?> UpdateTransactionAsync(int id, UpdateTransactionDto transaction);
        Task<bool> DeleteTransactionAsync(int id);
        Task<List<TransactionDto>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<TransactionDto>> GetTransactionsByTypeAsync(TransactionType type);
        Task<decimal> GetSumByTypeAndDateRangeAsync(TransactionType type, DateTime startDate, DateTime endDate);
        Task<DashboardDto> GetDashboardDataAsync();
    }
}
