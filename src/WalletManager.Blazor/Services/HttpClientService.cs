using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace WalletManager.Blazor.Services
{
    public class HttpClientService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;

        public HttpClientService(
            HttpClient httpClient,
            ILocalStorageService localStorage,
            NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.GetAsync(url);
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _navigationManager.NavigateTo("/login");
                    return default;
                }
                
                await EnsureSuccessStatusCode(response);
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<T?> PostAsync<T>(string url, object data)
        {
            try
            {
                await AddAuthorizationHeader();
                var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _navigationManager.NavigateTo("/login");
                    return default;
                }
                
                await EnsureSuccessStatusCode(response);
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<T?> PutAsync<T>(string url, object data)
        {
            try
            {
                await AddAuthorizationHeader();
                var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _navigationManager.NavigateTo("/login");
                    return default;
                }
                
                await EnsureSuccessStatusCode(response);
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<bool> DeleteAsync(string url)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.DeleteAsync(url);
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _navigationManager.NavigateTo("/login");
                    return false;
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task AddAuthorizationHeader()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"HTTP request failed with status code {response.StatusCode}: {content}");
            }
        }
    }
}
