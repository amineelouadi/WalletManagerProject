using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletManager.Blazor.Models;
using WalletManager.Blazor.Services.Interfaces;

namespace WalletManager.Blazor.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly HttpClientService _httpClient;

        public TransactionService(HttpClientService httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TransactionDto>> GetAllTransactionsAsync()
        {
            return await _httpClient.GetAsync<List<TransactionDto>>("api/transactions") ?? new List<TransactionDto>();
        }

        public async Task<List<TransactionDto>> GetTransactionsByWalletIdAsync(int walletId)
        {
            return await _httpClient.GetAsync<List<TransactionDto>>($"api/transactions/wallet/{walletId}") ?? new List<TransactionDto>();
        }

        public async Task<TransactionDto?> GetTransactionByIdAsync(int id)
        {
            return await _httpClient.GetAsync<TransactionDto>($"api/transactions/{id}");
        }

        public async Task<TransactionDto?> CreateTransactionAsync(CreateTransactionDto transaction)
        {
            return await _httpClient.PostAsync<TransactionDto>("api/transactions", transaction);
        }

        public async Task<TransactionDto?> UpdateTransactionAsync(int id, UpdateTransactionDto transaction)
        {
            try
            {
                return await _httpClient.PutAsync<TransactionDto>($"api/transactions/{id}", transaction);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                Console.WriteLine("Access denied: You are not authorized to update transactions.");
                return null;
            }
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            try
            {
                return await _httpClient.DeleteAsync($"api/transactions/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                Console.WriteLine("Access denied: You are not authorized to delete transactions.");
                return false;
            }
        }

        public async Task<List<TransactionDto>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _httpClient.GetAsync<List<TransactionDto>>($"api/transactions/date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}") ?? new List<TransactionDto>();
        }

        public async Task<List<TransactionDto>> GetTransactionsByTypeAsync(TransactionType type)
        {
            return await _httpClient.GetAsync<List<TransactionDto>>($"api/transactions/type/{(int)type}") ?? new List<TransactionDto>();
        }

        public async Task<decimal> GetSumByTypeAndDateRangeAsync(TransactionType type, DateTime startDate, DateTime endDate)
        {
            var result = await _httpClient.GetAsync<SumResponse>($"api/transactions/sum-by-type?type={(int)type}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            return result?.Sum ?? 0;
        }

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            return await _httpClient.GetAsync<DashboardDto>("https://localhost:52041/api/dashboard") ?? new DashboardDto();
        }
    }

    internal class SumResponse
    {
        public decimal Sum { get; set; }
    }
}
