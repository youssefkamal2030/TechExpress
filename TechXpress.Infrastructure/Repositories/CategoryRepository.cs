using Microsoft.EntityFrameworkCore;
using TechXpress.Application.Interfaces;
using TechXpress.Domain.Entities;
using TechXpress.Infrastructure.Data;

namespace TechXpress.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TechXpressDbContext _context;

        public CategoryRepository(TechXpressDbContext context)
        {
            _context = context;
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
        }
    }
}