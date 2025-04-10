using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletManager.Application.DTOs;
using WalletManager.Domain.Enums;

namespace WalletManager.Application.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync(string userId);
        Task<IEnumerable<TransactionDto>> GetTransactionsByWalletIdAsync(int walletId, string userId);
        Task<TransactionDto?> GetTransactionByIdAsync(int id, string userId);
        Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto createTransactionDto, string userId);
        Task<TransactionDto?> UpdateTransactionAsync(int id, UpdateTransactionDto updateTransactionDto, string userId);
        Task<bool> DeleteTransactionAsync(int id, string userId);
        Task<IEnumerable<TransactionDto>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate, string userId);
        Task<IEnumerable<TransactionDto>> GetTransactionsByTypeAsync(TransactionType type, string userId);
        Task<decimal> GetSumByTypeAndDateRangeAsync(TransactionType type, DateTime startDate, DateTime endDate, string userId);
        Task<DashboardDto> GetDashboardDataAsync(string userId);
    }
}
