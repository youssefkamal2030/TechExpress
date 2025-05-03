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
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly TechXpressDbContext _context;

        public ShoppingCartRepository(TechXpressDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ShoppingCart> GetCartWithItemsAsync(string userId)
        {
            return await _context.ShoppingCarts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
        
        public async Task<ShoppingCart> GetCartByUserIdAsync(string userId)
        {
            return await GetCartWithItemsAsync(userId);
        }
        
        public async Task<bool> AddItemToCartAsync(string userId, int productId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than zero");
                }
                
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    throw new ArgumentException("Product not found");
                }
                
                var cart = await GetCartWithItemsAsync(userId);
                if (cart == null)
                {
                    cart = new ShoppingCart { UserId = userId };
                    await _context.ShoppingCarts.AddAsync(cart);
                    await _context.SaveChangesAsync();
                }

                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    var newItem = new CartItem
                    {
                        ShoppingCartId = cart.Id,
                        ProductId = productId,
                        Quantity = quantity,
                        PriceAtAdd = product.Price
                    };
                    await _context.CartItems.AddAsync(newItem);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        public async Task<bool> RemoveItemFromCartAsync(string userId, int productId)
        {
            try
            {
                var cart = await GetCartWithItemsAsync(userId);
                if (cart == null)
                {
                    return false;
                }

                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    _context.CartItems.Remove(item);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            try
            {
                var cart = await GetCartWithItemsAsync(userId);
                if (cart == null)
                {
                    return false;
                }

                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public new async Task<IEnumerable<ShoppingCart>> FindAsync(Expression<Func<ShoppingCart, bool>> predicate)
        {
            return await _context.ShoppingCarts.Where(predicate).ToListAsync();
        }
    }
} 