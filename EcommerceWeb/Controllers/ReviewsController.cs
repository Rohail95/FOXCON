using BusinessLayer.Services;
using DatabaseLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EcommerceWeb.Controllers
{
    [Authorize] // Only logged-in users can leave a review
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICustomerService _customerService;

        public ReviewsController(
            IReviewService reviewService,
            UserManager<ApplicationUser> userManager,
            ICustomerService customerService)
        {
            _reviewService = reviewService;
            _userManager = userManager;
            _customerService = customerService;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId, int rating, string comment)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.Email == null) return Unauthorized();

                var customer = await _customerService.GetCustomerByEmailAsync(user.Email);
                if (customer == null) return Unauthorized();

                var review = new Review
                {
                    ProductId = productId,
                    CustomerId = customer.CustomerId,
                    Rating = rating,
                    Comment = comment,
                    ReviewDate = DateTime.UtcNow,
                    Status = "Pending" // Awaiting admin approval
                };

                await _reviewService.CreateReviewAsync(review);
                TempData["SuccessMessage"] = "Thank you! Your review has been submitted for approval.";
            }

            // Redirect back to the product page where the review was left
            return RedirectToAction("Details", "Products", new { id = productId });
        }
    }
}