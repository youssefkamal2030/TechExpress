using TechXpress.Models.entitis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechXpress.DataAccess.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetAllWithUserAndItemsAsync();
        Task<Order> GetByIdWithItemsAsync(int id);
    }
}