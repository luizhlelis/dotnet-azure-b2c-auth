using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DotnetAzureB2C.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("login")]
        public async Task Login()
        {
            await HttpContext
                .ChallengeAsync(
                    "AzureB2C",
                    new AuthenticationProperties() { RedirectUri = _configuration["AuthorizationServer:RedirectUri"] }
                );
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(
                new
                {
                    Name = User.Identity.Name,
                    EmailAddress = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value,
                    ProfileImage = User.FindFirst(c => c.Type == "picture")?.Value
                }
            );
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync("AzureB2C", new AuthenticationProperties
            {
                // Indicate here where AzureB2C should redirect the user after a logout.
                // Note that the resulting absolute Uri must be whitelisted in the 
                // **Allowed Logout URLs** settings for the client.
                RedirectUri = Url.Action("Index", "Home")
            });
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
