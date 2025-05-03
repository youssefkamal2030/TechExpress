using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Models.Dto_s;
using TechXpress.Models.entitis;
namespace TechXpress.Services.Interfaces
{
    public interface IOrderService
    {
        Task<bool> CreateOrderAsync(OrderDTO orderDto);
        Task<bool> UpdateOrderAsync(OrderDTO orderDto);
        Task<bool> DeleteOrderAsync(int id);
        Task<OrderDTO> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();
        Task<OrderDTO> PlaceOrderAsync(string userId, List<CartItemDTO> cartItems, string paymentToken);
        Task<IEnumerable<OrderDTO>> GetOrdersByUserIdAsync(string userId);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<IEnumerable<OrderDTO>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<OrderDTO>> GetOrdersByStatusAsync(OrderStatus status);
    }
}
