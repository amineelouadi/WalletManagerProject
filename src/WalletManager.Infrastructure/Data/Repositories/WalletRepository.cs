using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalletManager.Domain.Entities;
using WalletManager.Domain.Interfaces;

namespace WalletManager.Infrastructure.Data.Repositories
{
    public class WalletRepository : Repository<Wallet>, IWalletRepository
    {
        public WalletRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Wallet>> GetWalletsByUserIdAsync(string userId)
        {
            return await _dbSet
                .Include(w => w.Transactions)
                .Where(w => w.UserId == userId)
                .ToListAsync();
        }

        public async Task<Wallet?> GetWalletWithTransactionsAsync(int walletId)
        {
            return await _dbSet
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w => w.Id == walletId);
        }

        public async Task<decimal> GetTotalBalanceByUserIdAsync(string userId)
        {
            return await _dbSet
                .Where(w => w.UserId == userId)
                .SumAsync(w => w.Balance);
        }
    }
}
