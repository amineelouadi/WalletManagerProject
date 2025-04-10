using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WalletManager.Domain.Entities;
using WalletManager.Domain.Interfaces;

namespace WalletManager.Infrastructure.Identity
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, List<RefreshToken>> _refreshTokens;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
            _refreshTokens = new Dictionary<string, List<RefreshToken>>();
        }

        public (string AccessToken, string RefreshToken, int ExpiresIn) GenerateToken(User user, IList<string> roles)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryInMinutes = Convert.ToInt32(jwtSettings["ExpiryInMinutes"]);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username", user.UserName)
            };

            // Add roles as claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenExpiryTime = DateTime.UtcNow.AddMinutes(expiryInMinutes);
            var signingKey = new SymmetricSecurityKey(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpiryTime,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                Issuer = issuer,
                Audience = audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            // Store refresh token
            StoreRefreshToken(user.Id, refreshToken);

            return (accessToken, refreshToken, expiryInMinutes * 60);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateLifetime = false // Don't validate the expiry time because the token might be expired
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                
                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public bool ValidateRefreshToken(string userId, string refreshToken)
        {
            if (!_refreshTokens.ContainsKey(userId))
            {
                return false;
            }

            var userRefreshTokens = _refreshTokens[userId];
            var token = userRefreshTokens.FirstOrDefault(t => t.Token == refreshToken && t.Expires > DateTime.UtcNow);
            
            return token != null;
        }

        public void RevokeRefreshTokens(string userId)
        {
            if (_refreshTokens.ContainsKey(userId))
            {
                _refreshTokens.Remove(userId);
            }
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private void StoreRefreshToken(string userId, string token)
        {
            var refreshToken = new RefreshToken
            {
                Token = token,
                Expires = DateTime.UtcNow.AddDays(7) // Refresh token valid for 7 days
            };

            if (!_refreshTokens.ContainsKey(userId))
            {
                _refreshTokens[userId] = new List<RefreshToken>();
            }

            // Clean expired tokens
            _refreshTokens[userId].RemoveAll(t => t.Expires <= DateTime.UtcNow);
            
            // Add the new token
            _refreshTokens[userId].Add(refreshToken);
        }

        private class RefreshToken
        {
            public string Token { get; set; } = default!;
            public DateTime Expires { get; set; }
        }
    }
}
