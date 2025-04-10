using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalletManager.Application.DTOs;
using WalletManager.Application.Services.Interfaces;
using WalletManager.Domain.Enums;

namespace WalletManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var transactions = await _transactionService.GetAllTransactionsAsync(userId);
            return Ok(transactions);
        }

        [HttpGet("wallet/{walletId}")]
        public async Task<IActionResult> GetTransactionsByWalletId(int walletId)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var transactions = await _transactionService.GetTransactionsByWalletIdAsync(walletId, userId);
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var transaction = await _transactionService.GetTransactionByIdAsync(id, userId);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction(CreateTransactionDto createTransactionDto)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var transaction = await _transactionService.CreateTransactionAsync(createTransactionDto, userId);
                return CreatedAtAction(nameof(GetTransactionById), new { id = transaction.Id }, transaction);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, UpdateTransactionDto updateTransactionDto)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var transaction = await _transactionService.UpdateTransactionAsync(id, updateTransactionDto, userId);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _transactionService.DeleteTransactionAsync(id, userId);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetTransactionsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var transactions = await _transactionService.GetTransactionsByDateRangeAsync(startDate, endDate, userId);
            return Ok(transactions);
        }

        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetTransactionsByType(TransactionType type)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var transactions = await _transactionService.GetTransactionsByTypeAsync(type, userId);
            return Ok(transactions);
        }

        [HttpGet("sum-by-type")]
        public async Task<IActionResult> GetSumByTypeAndDateRange(
            [FromQuery] TransactionType type, 
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var sum = await _transactionService.GetSumByTypeAndDateRangeAsync(type, startDate, endDate, userId);
            return Ok(new { Sum = sum });
        }
    }
}
