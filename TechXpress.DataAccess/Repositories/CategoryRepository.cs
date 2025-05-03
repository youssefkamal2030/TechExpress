using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechXpress.DataAccess.Data;
using TechXpress.DataAccess.Interfaces;
using TechXpress.Models.entitis;

namespace TechXpress.DataAccess.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly TechXpressDbContext _context;

        public CategoryRepository(TechXpressDbContext context) : base(context)
        {
            _context = context;
        }

        public new async Task<IEnumerable<Category>> FindAsync(Expression<Func<Category, bool>> predicate)
        {
            return await _context.Categories.Where(predicate).ToListAsync();
        }
    }
} 