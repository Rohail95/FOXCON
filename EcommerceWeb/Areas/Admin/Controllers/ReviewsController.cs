using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EcommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // GET: /Admin/Reviews
        public async Task<IActionResult> Index()
        {
            // Use our new service method to get all the data
            var reviews = await _reviewService.GetAllReviewsWithDetailsAsync();
            return View(reviews);

        }

        // POST: /Admin/Reviews/Approve/5
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review != null)
            {
                review.Status = "Approved";
                await _reviewService.UpdateReviewAsync(review);
                TempData["SuccessMessage"] = "Review has been approved.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Reviews/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _reviewService.DeleteReviewAsync(id);
            TempData["SuccessMessage"] = "Review has been deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}