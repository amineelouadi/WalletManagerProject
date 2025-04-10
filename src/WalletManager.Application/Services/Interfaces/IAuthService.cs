using System.Threading.Tasks;
using WalletManager.Application.DTOs;

namespace WalletManager.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
        Task<AuthResultDto> RegisterAsync(RegisterUserDto registerDto);
        Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<bool> LogoutAsync(string userId);
        Task<UserDto?> GetCurrentUserAsync(string userId);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
    }
}
