using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalletManager.Application.DTOs;
using WalletManager.Application.Services.Interfaces;
using WalletManager.Domain.Enums;
using System.Security.Claims;
using System;

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

        private string GetUserId()
        {
            // Match WalletsController's user ID resolution
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            // Debug logging
            Console.WriteLine("Authorization header: " + Request.Headers["Authorization"]);
            LogClaims();

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID not found in claims");
                return Unauthorized();
            }

            Console.WriteLine($"Getting transactions for user: {userId}");
            var transactions = await _transactionService.GetAllTransactionsAsync(userId);
            return Ok(transactions);
        }

        [HttpGet("wallet/{walletId}")]
        public async Task<IActionResult> GetTransactionsByWalletId(int walletId)
        {
            Console.WriteLine("Authorization header: " + Request.Headers["Authorization"]);
            LogClaims();

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID not found in claims");
                return Unauthorized();
            }

            Console.WriteLine($"Getting transactions for wallet {walletId}, user: {userId}");
            var transactions = await _transactionService.GetTransactionsByWalletIdAsync(walletId, userId);
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            Console.WriteLine("Authorization header: " + Request.Headers["Authorization"]);
            LogClaims();

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID not found in claims");
                return Unauthorized();
            }

            Console.WriteLine($"Getting transaction {id} for user: {userId}");
            var transaction = await _transactionService.GetTransactionByIdAsync(id, userId);
            return transaction == null ? NotFound() : Ok(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction(CreateTransactionDto createTransactionDto)
        {
            Console.WriteLine("Authorization header: " + Request.Headers["Authorization"]);
            LogClaims();

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID not found in claims");
                return Unauthorized();
            }

            Console.WriteLine($"Creating transaction for user: {userId}");
            try
            {
                var transaction = await _transactionService.CreateTransactionAsync(createTransactionDto, userId);
                return CreatedAtAction(nameof(GetTransactionById), new { id = transaction.Id }, transaction);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Authorization error: {ex.Message}");
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating transaction: {ex}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, UpdateTransactionDto updateTransactionDto)
        {
            Console.WriteLine("Authorization header: " + Request.Headers["Authorization"]);
            LogClaims();

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID not found in claims");
                return Unauthorized();
            }

            Console.WriteLine($"Updating transaction {id} for user: {userId}");
            var transaction = await _transactionService.UpdateTransactionAsync(id, updateTransactionDto, userId);
            return transaction == null ? NotFound() : Ok(transaction);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            Console.WriteLine("Authorization header: " + Request.Headers["Authorization"]);
            LogClaims();

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID not found in claims");
                return Unauthorized();
            }

            Console.WriteLine($"Deleting transaction {id} for user: {userId}");
            var result = await _transactionService.DeleteTransactionAsync(id, userId);
            return result ? NoContent() : NotFound();
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetTransactionsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            Console.WriteLine("Authorization header: " + Request.Headers["Authorization"]);
            LogClaims();

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID not found in claims");
                return Unauthorized();
            }

            Console.WriteLine($"Getting transactions between {startDate} and {endDate} for user: {userId}");
            var transactions = await _transactionService.GetTransactionsByDateRangeAsync(startDate, endDate, userId);
            return Ok(transactions);
        }

        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetTransactionsByType(TransactionType type)
        {
            Console.WriteLine("Authorization header: " + Request.Headers["Authorization"]);
            LogClaims();

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID not found in claims");
                return Unauthorized();
            }

            Console.WriteLine($"Getting {type} transactions for user: {userId}");
            var transactions = await _transactionService.GetTransactionsByTypeAsync(type, userId);
            return Ok(transactions);
        }

        [HttpGet("sum-by-type")]
        public async Task<IActionResult> GetSumByTypeAndDateRange(
            [FromQuery] TransactionType type,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            Console.WriteLine("Authorization header: " + Request.Headers["Authorization"]);
            LogClaims();

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID not found in claims");
                return Unauthorized();
            }

            Console.WriteLine($"Calculating {type} sum between {startDate} and {endDate} for user: {userId}");
            var sum = await _transactionService.GetSumByTypeAndDateRangeAsync(type, startDate, endDate, userId);
            return Ok(new { Sum = sum });
        }

        private void LogClaims()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                Console.WriteLine("Authenticated user claims:");
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                }
            }
            else
            {
                Console.WriteLine("User is not authenticated");
            }
        }
    }
}