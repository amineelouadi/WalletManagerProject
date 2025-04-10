using System.Collections.Generic;
using System.Threading.Tasks;
using WalletManager.Blazor.Models;
using WalletManager.Blazor.Services.Interfaces;

namespace WalletManager.Blazor.Services
{
    public class WalletService : IWalletService
    {
        private readonly HttpClientService _httpClient;

        public WalletService(HttpClientService httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<WalletDto>> GetAllWalletsAsync()
        {
            return await _httpClient.GetAsync<List<WalletDto>>("api/wallets") ?? new List<WalletDto>();
        }

        public async Task<WalletDto?> GetWalletByIdAsync(int id)
        {
            return await _httpClient.GetAsync<WalletDto>($"api/wallets/{id}");
        }

        public async Task<WalletDto?> CreateWalletAsync(CreateWalletDto wallet)
        {
            return await _httpClient.PostAsync<WalletDto>("api/wallets", wallet);
        }

        public async Task<WalletDto?> UpdateWalletAsync(int id, UpdateWalletDto wallet)
        {
            return await _httpClient.PutAsync<WalletDto>($"api/wallets/{id}", wallet);
        }

        public async Task<bool> DeleteWalletAsync(int id)
        {
            return await _httpClient.DeleteAsync($"api/wallets/{id}");
        }

        public async Task<decimal> GetTotalBalanceAsync()
        {
            var result = await _httpClient.GetAsync<TotalBalanceResponse>("api/wallets/total-balance");
            return result?.TotalBalance ?? 0;
        }
    }

    internal class TotalBalanceResponse
    {
        public decimal TotalBalance { get; set; }
    }
}
