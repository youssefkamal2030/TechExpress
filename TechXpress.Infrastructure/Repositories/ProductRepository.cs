using Microsoft.EntityFrameworkCore;
using TechXpress.Application.Interfaces;
using TechXpress.Domain.Entities;
using TechXpress.Infrastructure.Data;

namespace TechXpress.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly TechXpressDbContext _context;

        public ProductRepository(TechXpressDbContext context)
        {
            _context = context;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
        }
    }
}