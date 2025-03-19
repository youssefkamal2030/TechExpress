using TechXpress.Models.entitis;
using TechXpress.Models.Dto_s;
namespace TechXpress.DataAccess.Interfaces
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        Task<ShoppingCart> GetCartByUserIdAsync(string userId);
        Task AddItemToCartAsync(string userId, int productId, int quantity);
        Task RemoveItemFromCartAsync(string userId, int productId);
        Task ClearCartAsync(string userId);
    }
}
