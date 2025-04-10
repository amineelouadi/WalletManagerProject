using System.ComponentModel.DataAnnotations;

namespace WalletManager.Application.DTOs
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; } = default!;
        
        [Required]
        public string Password { get; set; } = default!;
        
        public bool RememberMe { get; set; }
    }

    public class TokenDto
    {
        public string AccessToken { get; set; } = default!;
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; } = default!;
        public UserDto User { get; set; } = default!;
    }

    public class RefreshTokenDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }

    public class AuthResultDto
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public TokenDto? Token { get; set; }
    }
}
