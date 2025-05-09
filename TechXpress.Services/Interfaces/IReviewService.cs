using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress.Models.Dto_s;

namespace TechXpress.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync();
        Task<IEnumerable<ReviewDTO>> GetApprovedReviewsAsync();
        Task<IEnumerable<ReviewDTO>> GetProductReviewsAsync(int productId);
        Task<IEnumerable<ReviewDTO>> GetUserReviewsAsync(string userId);
        Task<ReviewDTO> GetReviewByIdAsync(int reviewId);
        Task AddReviewAsync(ReviewDTO reviewDto);
        Task UpdateReviewAsync(ReviewDTO reviewDto);
        Task DeleteReviewAsync(int reviewId);
        Task ApproveReviewAsync(int reviewId);
        Task<bool> HasUserReviewedProductAsync(string userId, int productId);
        Task<Dictionary<int, int>> GetRatingBreakdownAsync(int productId);
        Task<double> GetAverageRatingAsync(int productId);
    }
} 