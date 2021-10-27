using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using DotnetAzureB2C.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DotnetAzureB2C.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            // If the user is authenticated, then this is how you can get the access_token and id_token
            if (User.Identity.IsAuthenticated)
            {
                string accessToken = await HttpContext.GetTokenAsync("access_token");

                // if you need to check the access token expiration time, use this value
                // provided on the authorization response and stored.
                // do not attempt to inspect/decode the access token
                DateTime accessTokenExpiresAt = DateTime.Parse(
                    await HttpContext.GetTokenAsync("expires_at"),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind);

                string idToken = await HttpContext.GetTokenAsync("id_token");
            }

            return View();
        }

        [Authorize]
        public IActionResult Profile()
        {
            return View(new UserProfileViewModel()
            {
                Name = User.Identity.Name,
                EmailAddress = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value,
                ProfileImage = User.FindFirst(c => c.Type == "picture")?.Value
            });
        }
    }
}
