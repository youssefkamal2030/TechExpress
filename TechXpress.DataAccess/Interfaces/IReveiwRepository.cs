using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress.Models.entitis;

namespace TechXpress.DataAccess.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetAllWithDetailsAsync();
        Task<IEnumerable<Review>> GetProductReviewsAsync(int productId);
        Task<IEnumerable<Review>> GetUserReviewsAsync(string userId);
        Task<Review> GetReviewWithDetailsAsync(int reviewId);
        Task<bool> HasUserReviewedProductAsync(string userId, int productId);
    }
}