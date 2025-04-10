using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WalletManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("secure")]
        [Authorize] // Requires any valid token
        public IActionResult GetSecure() => Ok("Secure endpoint works!");

        [HttpGet("unsecure")]
        public IActionResult GetUnsecure() => Ok("Open endpoint works!");
    }
}
