#nullable disable

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using DatabaseLayer.Models;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Http;

namespace EcommerceWeb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager; // Add UserManager
        private readonly ILogger<LoginModel> _logger;
        private readonly ICartService _cartService;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, // Add UserManager
            ILogger<LoginModel> logger,
            ICartService cartService)
        {
            _signInManager = signInManager;
            _userManager = userManager; // Add UserManager
            _logger = logger;
            _cartService = cartService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            returnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ReturnUrl = returnUrl;
        }

     
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            var guestSessionId = HttpContext.Session.GetString("CartSessionId");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                
                var user = await _userManager.FindByEmailAsync(Input.Email);

                if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                {
                 
                    _logger.LogWarning("Admin login attempt blocked from public login page.");
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }

               
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    
                    if (!string.IsNullOrEmpty(guestSessionId) && user != null)
                    {
                        await _cartService.AssignCartToUserAsync(guestSessionId, user.Id);
                        HttpContext.Session.Remove("CartSessionId");
                    }

                    TempData["SuccessMessage"] = "You have been successfully logged in.";
                    return LocalRedirect(returnUrl);
                }

                
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                   
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            
            return Page();
        }
    }
}