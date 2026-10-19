using DatabaseLayer.Models;
using EcommerceWeb.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EcommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManageController : BaseAdminController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ManageController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Admin/Manage
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound("Unable to load user.");
            return View(user);
        }

        // GET: /Admin/Manage/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(AdminChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound("Unable to load user.");

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Your password has been changed successfully.";
            return RedirectToAction("Index");
        }

        // GET: /Admin/Manage/Email
        public async Task<IActionResult> Email()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound("Unable to load user.");

            var model = new AdminChangeEmailViewModel { NewEmail = user.Email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Email(AdminChangeEmailViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound("Unable to load user.");

            if (model.NewEmail != user.Email)
            {
                user.Email = model.NewEmail;
                user.UserName = model.NewEmail; // Also update UserName if it's based on email
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    // Handle errors...
                }

                // IMPORTANT: In a real app, you would send a confirmation email here.
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Your email has been changed successfully.";
                return RedirectToAction("Index");
            }
            TempData["SuccessMessage"] = "Your email is unchanged.";
            return RedirectToAction("Index");
        }
    }
}