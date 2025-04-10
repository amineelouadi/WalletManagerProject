using System;
using System.Collections.Generic;
using System.Security.Claims;
using WalletManager.Domain.Entities;

namespace WalletManager.Domain.Interfaces
{
    public interface IJwtTokenGenerator
    {
        (string AccessToken, string RefreshToken, int ExpiresIn) GenerateToken(User user, IList<string> roles);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        bool ValidateRefreshToken(string userId, string refreshToken);
        void RevokeRefreshTokens(string userId);
    }
}