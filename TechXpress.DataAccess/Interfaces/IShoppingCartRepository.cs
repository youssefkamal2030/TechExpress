using System.Threading.Tasks;
using TechXpress.Models.entitis;
using TechXpress.Models.Dto_s;
namespace TechXpress.DataAccess.Interfaces
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        Task<ShoppingCart> GetCartWithItemsAsync(string userId);
        Task<bool> AddItemToCartAsync(string userId, int productId, int quantity);
        Task<ShoppingCart> GetCartByUserIdAsync(string userId);

        Task<bool> RemoveItemFromCartAsync(string userId, int productId);
        Task<bool> ClearCartAsync(string userId);
    }
}
