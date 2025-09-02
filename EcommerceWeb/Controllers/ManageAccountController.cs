using BusinessLayer.Services;
using DatabaseLayer.Models;
using EcommerceWeb.ViewModels; // <-- ADD THIS
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EcommerceWeb.Controllers
{
    [Authorize]
    public class ManageAccountController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManageAccountController(
            ICustomerService customerService,
            UserManager<ApplicationUser> userManager)
        {
            _customerService = customerService;
            _userManager = userManager;
        }

        // GET: /ManageAccount
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var customer = await _customerService.GetCustomerByUserIdAsync(user.Id);
            if (customer == null) return NotFound("Customer profile not found.");

            // Create a ViewModel from the Customer data to send to the view
            var viewModel = new AccountUpdateViewModel
            {
                CustomerId = customer.CustomerId,
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

        // POST: /ManageAccount
        [HttpPost]
        public async Task<IActionResult> Index(AccountUpdateViewModel viewModel)
        {
            // Now, ModelState will only check the fields in the ViewModel, and will be VALID.
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();

                // Fetch the original, full customer record from the database.
                var customerToUpdate = await _customerService.GetCustomerByUserIdAsync(user.Id);
                if (customerToUpdate == null) return NotFound();

                // Security check
                if (customerToUpdate.CustomerId != viewModel.CustomerId) return Forbid();

                // Manually update the database object from the view model
                customerToUpdate.FirstName = viewModel.FirstName;
                customerToUpdate.LastName = viewModel.LastName;
                customerToUpdate.Phone = viewModel.Phone;
                customerToUpdate.Address = viewModel.Address;
                customerToUpdate.Address = viewModel.Address;
                customerToUpdate.City = viewModel.City;
                customerToUpdate.PostalCode = viewModel.PostalCode;
                customerToUpdate.Country = viewModel.Country;

                // Save the changes
                await _customerService.UpdateCustomerAsync(customerToUpdate);

                TempData["SuccessMessage"] = "Your profile has been updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            // If we get here, the form had an error (e.g. FirstName was blank)
            return View(viewModel);
        }
    }
}