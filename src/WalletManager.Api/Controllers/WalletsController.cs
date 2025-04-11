using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalletManager.Application.DTOs;
using WalletManager.Application.Services.Interfaces;

namespace WalletManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletsController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWallets()
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
            }

            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // Try with nameidentifier
                userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"Found nameidentifier claim: {userId}");

                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in claims");
                    return Unauthorized();
                }
            }

            Console.WriteLine($"Getting wallets for user: {userId}");
            var wallets = await _walletService.GetAllWalletsAsync(userId);
            return Ok(wallets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWalletById(int id)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // Try with nameidentifier
                userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
            }

            var wallet = await _walletService.GetWalletByIdAsync(id, userId);
            if (wallet == null)
            {
                return NotFound();
            }

            return Ok(wallet);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWallet(CreateWalletDto createWalletDto)
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
            }

            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // Try with nameidentifier
                userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"Found nameidentifier claim: {userId}");

                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in claims");
                    return Unauthorized();
                }
            }

            Console.WriteLine($"Creating wallet for user: {userId}");
            var wallet = await _walletService.CreateWalletAsync(createWalletDto, userId);
            return CreatedAtAction(nameof(GetWalletById), new { id = wallet.Id }, wallet);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWallet(int id, UpdateWalletDto updateWalletDto)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // Try with nameidentifier
                userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
            }

            var wallet = await _walletService.UpdateWalletAsync(id, updateWalletDto, userId);
            if (wallet == null)
            {
                return NotFound();
            }

            return Ok(wallet);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // Try with nameidentifier
                userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
            }

            var result = await _walletService.DeleteWalletAsync(id, userId);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("total-balance")]
        public async Task<IActionResult> GetTotalBalance()
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // Try with nameidentifier
                userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
            }

            var totalBalance = await _walletService.GetTotalBalanceAsync(userId);
            return Ok(new { TotalBalance = totalBalance });
        }
    }
}
