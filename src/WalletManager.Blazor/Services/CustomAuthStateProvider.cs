using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WalletManager.Blazor.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Parse the token to get the claims
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Create a claims identity based on the token
            var claims = jwtToken.Claims.ToList();
            var nameId = claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var userName = claims.FirstOrDefault(c => c.Type == "username")?.Value;
            var email = claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var role = claims.FirstOrDefault(c => c.Type == "role")?.Value;

            // Create a claims identity
            var identity = new ClaimsIdentity(claims, "jwt");
            
            // Create a claims principal
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}