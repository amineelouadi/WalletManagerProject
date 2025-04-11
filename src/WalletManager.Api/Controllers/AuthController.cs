using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalletManager.Application.DTOs;
using WalletManager.Application.Services.Interfaces;

namespace WalletManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = result.Message });
            }
            return Ok(result.Token);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerDto)
        {
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return BadRequest(new { message = "Passwords do not match." });
            }
            var result = await _authService.RegisterAsync(registerDto);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(result.Token);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenDto);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = result.Message });
            }
            return Ok(result.Token);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            await _authService.LogoutAsync(userId);
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpGet("current-user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var user = await _authService.GetCurrentUserAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            {
                return BadRequest(new { message = "New passwords do not match." });
            }
            var success = await _authService.ChangePasswordAsync(userId, changePasswordDto);
            if (!success)
            {
                return BadRequest(new { message = "Failed to change password. Make sure the current password is correct." });
            }
            return Ok(new { message = "Password changed successfully" });
        }

        // Helper method to get user ID from claims
        private string? GetUserId()
        {
            // Debug information
            Console.WriteLine("Authorization header: " + Request.Headers["Authorization"]);

            if (User.Identity?.IsAuthenticated == true)
            {
                Console.WriteLine("User is authenticated");
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
                }
            }
            else
            {
                Console.WriteLine("User is not authenticated");
                return null;
            }

            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // Try with nameidentifier
                userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"Found nameidentifier claim: {userId}");
            }

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID not found in claims");
            }
            else
            {
                Console.WriteLine($"User ID: {userId}");
            }

            return userId;
        }
    }
}