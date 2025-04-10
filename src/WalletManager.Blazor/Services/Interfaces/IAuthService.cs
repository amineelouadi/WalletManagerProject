using System.Threading.Tasks;

namespace WalletManager.Blazor.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(string username, string email, string password, string confirmPassword, string firstName, string lastName);
        Task<bool> LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string> GetUsernameAsync();
        Task<string> GetUserRoleAsync();
        Task<bool> ChangePasswordAsync(string currentPassword, string newPassword, string confirmNewPassword);
    }
}
