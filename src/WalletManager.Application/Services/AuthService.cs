using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using WalletManager.Application.DTOs;
using WalletManager.Application.Services.Interfaces;
using WalletManager.Domain.Entities;
using WalletManager.Domain.Enums;
using WalletManager.Domain.Interfaces;

namespace WalletManager.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;

        public AuthService(
            UserManager<User> userManager,
            IJwtTokenGenerator jwtTokenGenerator,
            IMapper mapper)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mapper = mapper;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null)
            {
                return new AuthResultDto
                {
                    Succeeded = false,
                    Message = "Invalid username or password."
                };
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                return new AuthResultDto
                {
                    Succeeded = false,
                    Message = "Invalid username or password."
                };
            }

            if (!user.IsActive)
            {
                return new AuthResultDto
                {
                    Succeeded = false,
                    Message = "Your account is disabled. Please contact support."
                };
            }

            // Update last login
            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Generate token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = roles.Count > 0 ? roles[0] : UserRole.User;

            return new AuthResultDto
            {
                Succeeded = true,
                Token = new TokenDto
                {
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken,
                    ExpiresIn = token.ExpiresIn,
                    User = userDto
                }
            };
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterUserDto registerDto)
        {
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return new AuthResultDto
                {
                    Succeeded = false,
                    Message = "Passwords do not match."
                };
            }

            var existingUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existingUser != null)
            {
                return new AuthResultDto
                {
                    Succeeded = false,
                    Message = "Username is already taken."
                };
            }

            existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new AuthResultDto
                {
                    Succeeded = false,
                    Message = "Email is already registered."
                };
            }

            var user = _mapper.Map<User>(registerDto);
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return new AuthResultDto
                {
                    Succeeded = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // Assign default role
            await _userManager.AddToRoleAsync(user, UserRole.User);

            // Generate token for the new user
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = UserRole.User;

            return new AuthResultDto
            {
                Succeeded = true,
                Token = new TokenDto
                {
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken,
                    ExpiresIn = token.ExpiresIn,
                    User = userDto
                }
            };
        }

        public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var principal = _jwtTokenGenerator.GetPrincipalFromExpiredToken(refreshTokenDto.AccessToken);
            if (principal == null)
            {
                return new AuthResultDto
                {
                    Succeeded = false,
                    Message = "Invalid access token."
                };
            }

            var userId = principal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new AuthResultDto
                {
                    Succeeded = false,
                    Message = "Invalid access token."
                };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return new AuthResultDto
                {
                    Succeeded = false,
                    Message = "User not found or disabled."
                };
            }

            if (!_jwtTokenGenerator.ValidateRefreshToken(user.Id, refreshTokenDto.RefreshToken))
            {
                return new AuthResultDto
                {
                    Succeeded = false,
                    Message = "Invalid refresh token."
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = roles.Count > 0 ? roles[0] : UserRole.User;

            return new AuthResultDto
            {
                Succeeded = true,
                Token = new TokenDto
                {
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken,
                    ExpiresIn = token.ExpiresIn,
                    User = userDto
                }
            };
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            
            // Invalidate refresh tokens
            _jwtTokenGenerator.RevokeRefreshTokens(userId);
            return true;
        }

        public async Task<UserDto?> GetCurrentUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return null;
            }

            var userDto = _mapper.Map<UserDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Role = roles.Count > 0 ? roles[0] : UserRole.User;

            return userDto;
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            {
                return false;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return false;
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            return result.Succeeded;
        }
    }
}
