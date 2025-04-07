using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechXpress.DataAccess.Data;
using TechXpress.DataAccess.Interfaces;
using TechXpress.Models.entitis;

namespace TechXpress.DataAccess.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly TechXpressDbContext _context;

        public ShoppingCartRepository(TechXpressDbContext context)
        {
            _context = context;
        }

        public async Task<ShoppingCart> GetByIdAsync(int id)
        {
            return await _context.ShoppingCarts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product) 
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<ShoppingCart>> GetAllAsync()
        {
            return await _context.ShoppingCarts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product) 
                .ToListAsync();
        }

        public async Task AddAsync(ShoppingCart entity)
        {
            await _context.ShoppingCarts.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ShoppingCart entity)
        {
            _context.ShoppingCarts.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cart = await _context.ShoppingCarts.FindAsync(id);
            if (cart != null)
            {
                _context.ShoppingCarts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }

        // IShoppingCartRepository Methods
        public async Task<ShoppingCart> GetCartByUserIdAsync(string userId)
        {
            return await _context.ShoppingCarts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
        }

        public async Task AddItemToCartAsync(string userId, int productId, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            }

            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    ApplicationUserId = userId,
                    Items = new List<CartItem>()
                };
                await _context.ShoppingCarts.AddAsync(cart);
            }

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    throw new ArgumentException("Product not found.", nameof(productId));
                }
                cart.Items.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    PriceAtAdd = product.Price // Capture price at the time of addition
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemFromCartAsync(string userId, int productId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null) return;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cart.Items.Remove(item);
                if (!cart.Items.Any())
                {
                    _context.ShoppingCarts.Remove(cart); // Remove cart if empty
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if (cart != null)
            {
                _context.ShoppingCarts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }
    }
}