using TechXpress.Application.Interfaces;
using TechXpress.Domain.Entities;
using TechXpress.Infrastructure.Repositories;

namespace TechXpress.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TechXpressDbContext _context;

        public UnitOfWork(TechXpressDbContext context)
        {
            _context = context;
            Orders = new OrderRepository(_context);
            Products = new ProductRepository(_context);
            Categories = new CategoryRepository(_context);
        }

        public IOrderRepository Orders { get; }
        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}