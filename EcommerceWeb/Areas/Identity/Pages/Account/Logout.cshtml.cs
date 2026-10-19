#nullable disable

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DatabaseLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace EcommerceWeb.Areas.Identity.Pages.Account
{
    [AllowAnonymous] // Allow anyone to access the logout page
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet(string returnUrl = null)
        {
            // The logic is the same as OnPost: sign out and redirect.
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out via GET request.");

            // The 'returnUrl' from _LoginPartial is the homepage ("/")
            returnUrl ??= Url.Content("~/");
            return LocalRedirect(returnUrl);
        }

        // This existing method handles POST requests (from forms)
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out via POST request.");

            returnUrl ??= Url.Content("~/");
            return LocalRedirect(returnUrl);
        }
    }
}