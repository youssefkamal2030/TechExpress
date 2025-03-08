using TechXpress.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechXpress.Application.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(int productId, int quantity);
        Task RemoveFromCartAsync(int productId);
        Task<IEnumerable<CartItem>> GetCartItemsAsync();
        Task ClearCartAsync();
    }
}