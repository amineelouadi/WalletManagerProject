using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalletManager.Application.Services.Interfaces;

namespace WalletManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public DashboardController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
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

            Console.WriteLine($"Getting dashboard data for user: {userId}");
            var dashboardData = await _transactionService.GetDashboardDataAsync(userId);
            return Ok(dashboardData);
        }
    }
}