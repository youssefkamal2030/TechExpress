using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechXpress.DataAccess.Data;
using TechXpress.Models.entitis; 
using TechXpress.DataAccess.Interfaces;

namespace TechXpress.DataAccess.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly TechXpressDbContext _context;

        public ShoppingCartRepository(TechXpressDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ShoppingCart entity)
        {
            if (entity != null)
            {
                await _context.shoppingCarts.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var cartItem = await _context.shoppingCarts.FindAsync(id);
            if (cartItem != null)
            {
                _context.shoppingCarts.Remove(cartItem);
            }
        }

        public async Task<IEnumerable<ShoppingCart>> GetAllAsync()
        {
            return await _context.shoppingCarts.ToListAsync();
        }

        public async Task<ShoppingCart> GetByIdAsync(int id)
        {
            return await _context.shoppingCarts.FindAsync(id);
        }

        public async Task UpdateAsync(ShoppingCart entity)
        {
            if (entity != null)
            {
                _context.shoppingCarts.Update(entity);
            }
        }
    }
}