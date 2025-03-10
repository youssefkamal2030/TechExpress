using TechXpress.Models.entitis;

namespace TechXpress.DataAccess.Interfaces
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        Task<ShoppingCart> GetByUserIdAndProductIdAsync(string userId, int productId);
        Task<IEnumerable<ShoppingCart>> GetByUserIdAsync(string userId);
    }
}
