using TechXpress.Domain.Entities;

namespace TechXpress.Application.Interfaces
{
    public interface ICartStorage
    {
        Task SaveCartAsync(List<CartItem> cartItems);
        Task<List<CartItem>> GetCartAsync();
        Task ClearCartAsync();
    }
}