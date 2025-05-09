using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TechXpress.Models.Dto_s;
using TechXpress.Services.Interfaces;

namespace TechXpress.Web.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IProductService _productService;

        public ReviewController(IReviewService reviewService, IProductService productService)
        {
            _reviewService = reviewService;
            _productService = productService;
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReview(ReviewDTO review)
        {
            try
            {
                review.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                review.UserName = User.Identity.Name;
                review.CreatedAt = DateTime.UtcNow;
                review.IsApproved = true;
                await _reviewService.AddReviewAsync(review);
                TempData["SuccessMessage"] = "Your review has been submitted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Details", "Product", new { id = review.ProductId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReview(ReviewDTO review)
        {
            if (!ModelState.IsValid)
            {
                return View(review);
            }

            try
            {
                review.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _reviewService.UpdateReviewAsync(review);
                TempData["SuccessMessage"] = "Your review has been updated successfully!";
                return RedirectToAction("MyReviews");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(review);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int reviewId, int productId)
        {
            try
            {
                await _reviewService.DeleteReviewAsync(reviewId);
                TempData["SuccessMessage"] = "Your review has been deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Details", "Product", new { id = productId });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveReview(int reviewId, int productId)
        {
            try
            {
                await _reviewService.ApproveReviewAsync(reviewId);
                TempData["SuccessMessage"] = "Review has been approved successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Details", "Product", new { id = productId });
        }

        [Authorize]
        public async Task<IActionResult> MyReviews()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reviews = await _reviewService.GetUserReviewsAsync(userId);
            return View(reviews);
        }

        [Authorize]
        public async Task<IActionResult> EditReview(int id)
        {
            try
            {
                // Get the review by ID
                var review = await _reviewService.GetReviewByIdAsync(id);
                
                // Check if the review exists
                if (review == null)
                {
                    TempData["ErrorMessage"] = "Review not found.";
                    return RedirectToAction("MyReviews");
                }
                
                // Verify that the current user owns this review
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (review.UserId != userId && !User.IsInRole("Admin"))
                {
                    TempData["ErrorMessage"] = "You are not authorized to edit this review.";
                    return RedirectToAction("MyReviews");
                }
                
                return View(review);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("MyReviews");
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageReviews()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return View(reviews);
        }
    }
} 