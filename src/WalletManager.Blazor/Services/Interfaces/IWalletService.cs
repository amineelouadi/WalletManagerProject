using System.Collections.Generic;
using System.Threading.Tasks;
using WalletManager.Blazor.Models;

namespace WalletManager.Blazor.Services.Interfaces
{
    public interface IWalletService
    {
        Task<List<WalletDto>> GetAllWalletsAsync();
        Task<WalletDto?> GetWalletByIdAsync(int id);
        Task<WalletDto?> CreateWalletAsync(CreateWalletDto wallet);
        Task<WalletDto?> UpdateWalletAsync(int id, UpdateWalletDto wallet);
        Task<bool> DeleteWalletAsync(int id);
        Task<decimal> GetTotalBalanceAsync();
    }
}
