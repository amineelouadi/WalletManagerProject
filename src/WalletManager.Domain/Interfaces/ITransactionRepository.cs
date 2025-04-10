using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletManager.Domain.Entities;
using WalletManager.Domain.Enums;

namespace WalletManager.Domain.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetTransactionsByWalletIdAsync(int walletId);
        Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(string userId);
        Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate, string userId);
        Task<IEnumerable<Transaction>> GetTransactionsByTypeAsync(TransactionType type, string userId);
        Task<decimal> GetSumByTypeAndDateRangeAsync(TransactionType type, DateTime startDate, DateTime endDate, string userId);
    }
}
