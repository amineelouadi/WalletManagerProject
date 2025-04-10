using System;
using System.Threading.Tasks;
using WalletManager.Domain.Interfaces;
using WalletManager.Infrastructure.Data.Repositories;

namespace WalletManager.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IWalletRepository? _walletRepository;
        private ITransactionRepository? _transactionRepository;
        private bool _disposed = false;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IWalletRepository Wallets => _walletRepository ??= new WalletRepository(_context);

        public ITransactionRepository Transactions => _transactionRepository ??= new TransactionRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
