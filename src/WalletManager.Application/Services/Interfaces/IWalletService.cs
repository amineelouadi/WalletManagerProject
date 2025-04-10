using System.Collections.Generic;
using System.Threading.Tasks;
using WalletManager.Application.DTOs;

namespace WalletManager.Application.Services.Interfaces
{
    public interface IWalletService
    {
        Task<IEnumerable<WalletDto>> GetAllWalletsAsync(string userId);
        Task<WalletDto?> GetWalletByIdAsync(int id, string userId);
        Task<WalletDto> CreateWalletAsync(CreateWalletDto createWalletDto, string userId);
        Task<WalletDto?> UpdateWalletAsync(int id, UpdateWalletDto updateWalletDto, string userId);
        Task<bool> DeleteWalletAsync(int id, string userId);
        Task<decimal> GetTotalBalanceAsync(string userId);
    }
}
