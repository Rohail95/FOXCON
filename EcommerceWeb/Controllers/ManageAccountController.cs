using BusinessLayer.Services;
using DatabaseLayer.Models;
using EcommerceWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using System.Threading.Tasks;

namespace EcommerceWeb.Controllers
{
    [Authorize]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ManageAccountController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ManageAccountController(
            ICustomerService customerService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _customerService = customerService;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var customer = await _customerService.GetCustomerByUserIdAsync(user.Id);
            if (customer == null)
            {
                return NotFound("Customer profile not found for this user.");
            }

            var viewModel = new UserProfileViewModel
            {
                ExistingImageUrl = user.ProfileImageUrl,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                Address = customer.Address,
                City = customer.City,
                PostalCode = customer.PostalCode,
                Country = customer.Country
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserProfileViewModel viewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // Re-populate existing image URL in case of validation error
            viewModel.ExistingImageUrl = user.ProfileImageUrl;

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var customerToUpdate = await _customerService.GetCustomerByUserIdAsync(user.Id);
            if (customerToUpdate == null)
            {
                return NotFound();
            }

            if (viewModel.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/profiles");
                Directory.CreateDirectory(uploadsFolder);
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.ProfileImage.CopyToAsync(fileStream);
                }

                user.ProfileImageUrl = "/images/profiles/" + uniqueFileName.Replace("\\", "/");
                await _userManager.UpdateAsync(user);
            }

            customerToUpdate.FirstName = viewModel.FirstName;
            customerToUpdate.LastName = viewModel.LastName;
            customerToUpdate.Phone = viewModel.Phone;
            customerToUpdate.Address = viewModel.Address;
            customerToUpdate.City = viewModel.City;
            customerToUpdate.PostalCode = viewModel.PostalCode;
            customerToUpdate.Country = viewModel.Country;

            await _customerService.UpdateCustomerAsync(customerToUpdate);

            TempData["SuccessMessage"] = "Your profile has been updated successfully!";

            return RedirectToAction(nameof(Index));
        }
    }
}