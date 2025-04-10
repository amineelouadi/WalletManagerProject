using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using WalletManager.Blazor.Models;
using WalletManager.Blazor.Services.Interfaces;

namespace WalletManager.Blazor.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(
            HttpClient httpClient,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var loginModel = new LoginRequest
                {
                    UserName = username,
                    Password = password
                };

                var response = await _httpClient.PostAsJsonAsync("https://localhost:52041/api/Auth/login", loginModel);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
                if (result == null)
                {
                    return false;
                }

                // Store the token in local storage
                await _localStorage.SetItemAsync("authToken", result.AccessToken);
                await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);
                await _localStorage.SetItemAsync("userName", result.User.UserName);
                await _localStorage.SetItemAsync("fullName", $"{result.User.FirstName} {result.User.LastName}");
                await _localStorage.SetItemAsync("userRole", result.User.Role);

                // Notify the auth state has changed
                ((CustomAuthStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RegisterAsync(string username, string email, string password, string confirmPassword, string firstName, string lastName)
        {
            try
            {
                var registerModel = new RegisterRequest
                {
                    UserName = username,
                    Email = email,
                    Password = password,
                    ConfirmPassword = confirmPassword,
                    FirstName = firstName,
                    LastName = lastName
                };

                var response = await _httpClient.PostAsJsonAsync("https://localhost:52041/api/auth/register", registerModel);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
                if (result == null)
                {
                    return false;
                }

                // Store the token in local storage
                await _localStorage.SetItemAsync("authToken", result.AccessToken);
                await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);
                await _localStorage.SetItemAsync("userName", result.User.UserName);
                await _localStorage.SetItemAsync("fullName", $"{result.User.FirstName} {result.User.LastName}");
                await _localStorage.SetItemAsync("userRole", result.User.Role);

                // Notify the auth state has changed
                ((CustomAuthStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    await _httpClient.PostAsync("https://localhost:52041/api/auth/logout", null);
                }

                await _localStorage.RemoveItemAsync("authToken");
                await _localStorage.RemoveItemAsync("refreshToken");
                await _localStorage.RemoveItemAsync("userName");
                await _localStorage.RemoveItemAsync("fullName");
                await _localStorage.RemoveItemAsync("userRole");

                // Notify the auth state has changed
                ((CustomAuthStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string> GetUsernameAsync()
        {
            return await _localStorage.GetItemAsync<string>("userName") ?? string.Empty;
        }

        public async Task<string> GetUserRoleAsync()
        {
            return await _localStorage.GetItemAsync<string>("userRole") ?? string.Empty;
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword, string confirmNewPassword)
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (string.IsNullOrEmpty(token))
                {
                    return false;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var changePasswordModel = new ChangePasswordRequest
                {
                    CurrentPassword = currentPassword,
                    NewPassword = newPassword,
                    ConfirmNewPassword = confirmNewPassword
                };

                var response = await _httpClient.PostAsJsonAsync("api/auth/change-password", changePasswordModel);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
