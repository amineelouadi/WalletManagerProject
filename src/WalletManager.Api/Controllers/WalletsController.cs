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
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var wallets = await _walletService.GetAllWalletsAsync(userId);
            return Ok(wallets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWalletById(int id)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
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
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var wallet = await _walletService.CreateWalletAsync(createWalletDto, userId);
            return CreatedAtAction(nameof(GetWalletById), new { id = wallet.Id }, wallet);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWallet(int id, UpdateWalletDto updateWalletDto)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
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
                return Unauthorized();
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
                return Unauthorized();
            }

            var totalBalance = await _walletService.GetTotalBalanceAsync(userId);
            return Ok(new { TotalBalance = totalBalance });
        }
    }
}
