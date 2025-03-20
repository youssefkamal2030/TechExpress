using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Models.Dto_s;
using TechXpress.Models.entitis;

namespace TechXpress.Services.Interfaces
{
    public interface IShoppingCartService 
    {
        Task AddToCartAsync(int productId, int quantity, string userId);
        Task RemoveFromCartAsync(int productId, string userId);
        Task ClearCartAsync(string userId);
        Task<int> GetCartCountAsync(string userId);
        Task<decimal> GetCartTotalAsync(string userId);
        Task<ShoppingCartDTO> GetCartByUserIdAsync(string userId);
    }
}
