using Microsoft.EntityFrameworkCore;
using TechXpress.Application.Interfaces;
using TechXpress.Domain.Entities;
using TechXpress.Infrastructure.Data;

namespace TechXpress.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly TechXpressDbContext _context;

        public OrderRepository(TechXpressDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
        }

        public async Task DeleteAsync(int id)
        {
            var order = await GetByIdAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
        }
    }
}