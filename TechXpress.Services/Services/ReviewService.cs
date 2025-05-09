using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechXpress.DataAccess.Interfaces;
using TechXpress.Models.Dto_s;
using TechXpress.Models.entitis;
using TechXpress.Services.Interfaces;

namespace TechXpress.Services.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync()
        {
            var reviews = await _unitOfWork.Reviews.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
        }

        public async Task<IEnumerable<ReviewDTO>> GetApprovedReviewsAsync()
        {
            var allReviews = await _unitOfWork.Reviews.GetAllWithDetailsAsync();
            var approvedReviews = allReviews.Where(r => r.IsApproved);
            return _mapper.Map<IEnumerable<ReviewDTO>>(approvedReviews);
        }

        public async Task<IEnumerable<ReviewDTO>> GetProductReviewsAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetProductReviewsAsync(productId);
            var approvedReviews = reviews.Where(r => r.IsApproved);
            return _mapper.Map<IEnumerable<ReviewDTO>>(approvedReviews);
        }

        public async Task<IEnumerable<ReviewDTO>> GetUserReviewsAsync(string userId)
        {
            var reviews = await _unitOfWork.Reviews.GetUserReviewsAsync(userId);
            return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
        }

        public async Task<ReviewDTO> GetReviewByIdAsync(int reviewId)
        {
            var review = await _unitOfWork.Reviews.GetReviewWithDetailsAsync(reviewId);
            if (review == null)
            {
                throw new KeyNotFoundException($"Review with ID {reviewId} not found");
            }
            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task AddReviewAsync(ReviewDTO reviewDto)
        {
            // Check if product exists
            var product = await _unitOfWork.Products.GetByIdAsync(reviewDto.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {reviewDto.ProductId} not found");
            }

            // Check if user has already reviewed this product
            if (await HasUserReviewedProductAsync(reviewDto.UserId, reviewDto.ProductId))
            {
                throw new InvalidOperationException("You have already reviewed this product");
            }

            var review = _mapper.Map<Review>(reviewDto);
            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateReviewAsync(ReviewDTO reviewDto)
        {
            var existingReview = await _unitOfWork.Reviews.GetByIdAsync(reviewDto.Id);
            if (existingReview == null)
            {
                throw new KeyNotFoundException($"Review with ID {reviewDto.Id} not found");
            }

            // Ensure user can only edit their own reviews
            if (existingReview.UserId != reviewDto.UserId)
            {
                throw new UnauthorizedAccessException("You can only edit your own reviews");
            }

            _mapper.Map(reviewDto, existingReview);
            await _unitOfWork.Reviews.UpdateAsync(existingReview);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null)
            {
                throw new KeyNotFoundException($"Review with ID {reviewId} not found");
            }

            await _unitOfWork.Reviews.DeleteAsync(reviewId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ApproveReviewAsync(int reviewId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null)
            {
                throw new KeyNotFoundException($"Review with ID {reviewId} not found");
            }

            review.IsApproved = true;
            await _unitOfWork.Reviews.UpdateAsync(review);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> HasUserReviewedProductAsync(string userId, int productId)
        {
            return await _unitOfWork.Reviews.HasUserReviewedProductAsync(userId, productId);
        }

        public async Task<Dictionary<int, int>> GetRatingBreakdownAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetProductReviewsAsync(productId);
            var approvedReviews = reviews.Where(r => r.IsApproved).ToList();
            
            var breakdown = new Dictionary<int, int>();
            for (int i = 1; i <= 5; i++)
            {
                breakdown[i] = approvedReviews.Count(r => r.Rating == i);
            }
            
            return breakdown;
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetProductReviewsAsync(productId);
            var approvedReviews = reviews.Where(r => r.IsApproved).ToList();
            
            if (!approvedReviews.Any())
            {
                return 0;
            }
            
            return approvedReviews.Average(r => r.Rating);
        }
    }
} 