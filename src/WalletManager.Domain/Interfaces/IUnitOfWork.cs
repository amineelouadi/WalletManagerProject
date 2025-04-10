using System;
using System.Threading.Tasks;

namespace WalletManager.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IWalletRepository Wallets { get; }
        ITransactionRepository Transactions { get; }
        Task<int> SaveChangesAsync();
    }
}
