using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalletManager.Domain.Entities;
using WalletManager.Domain.Enums;
using WalletManager.Domain.Interfaces;

namespace WalletManager.Infrastructure.Data.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByWalletIdAsync(int walletId)
        {
            return await _dbSet
                .Include(t => t.Wallet)
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(string userId)
        {
            return await _dbSet
                .Include(t => t.Wallet)
                .Where(t => t.Wallet.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate, string userId)
        {
            return await _dbSet
                .Include(t => t.Wallet)
                .Where(t => t.Wallet.UserId == userId &&
                           t.TransactionDate >= startDate &&
                           t.TransactionDate <= endDate)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByTypeAsync(TransactionType type, string userId)
        {
            return await _dbSet
                .Include(t => t.Wallet)
                .Where(t => t.Wallet.UserId == userId && t.Type == type)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<decimal> GetSumByTypeAndDateRangeAsync(TransactionType type, DateTime startDate, DateTime endDate, string userId)
        {
            return await _dbSet
                .Where(t => t.Wallet.UserId == userId &&
                           t.Type == type &&
                           t.TransactionDate >= startDate &&
                           t.TransactionDate <= endDate)
                .SumAsync(t => t.Amount);
        }
    }
}
