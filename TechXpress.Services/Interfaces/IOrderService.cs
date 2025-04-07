using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Models.entitis;
using TechXpress.Models.Dto_s;
namespace TechXpress.Services.Interfaces
{
    public interface IOrderService
    {
        Task<bool> CreateOrderAsync(Order order);
        Task<bool> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int id);
        Task<Order> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> PlaceOrderAsync(string userId, List<CartItem> cartItems, string paymentToken);
        Task<IEnumerable<OrderDTO>> GetOrdersByUserIdAsync(string userId);
    }
}
