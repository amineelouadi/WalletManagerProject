using System.Collections.Generic;
using System.Threading.Tasks;
using WalletManager.Domain.Entities;

namespace WalletManager.Domain.Interfaces
{
    public interface IWalletRepository : IRepository<Wallet>
    {
        Task<IEnumerable<Wallet>> GetWalletsByUserIdAsync(string userId);
        Task<Wallet?> GetWalletWithTransactionsAsync(int walletId);
        Task<decimal> GetTotalBalanceByUserIdAsync(string userId);
    }
}
