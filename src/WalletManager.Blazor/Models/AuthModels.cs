using System.Text.Json.Serialization;

namespace WalletManager.Blazor.Models
{
    public class LoginRequest
    {
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public bool RememberMe { get; set; }
    }

    public class RegisterRequest
    {
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
        public string ConfirmNewPassword { get; set; } = default!;
    }

    public class UserDto
    {
        public string Id { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string? Role { get; set; }
    }

    public class TokenResponse
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; } = default!;

        [JsonPropertyName("tokenType")]
        public string TokenType { get; set; } = default!;

        [JsonPropertyName("expiresIn")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = default!;

        [JsonPropertyName("user")]
        public UserDto User { get; set; } = default!;
    }
}
