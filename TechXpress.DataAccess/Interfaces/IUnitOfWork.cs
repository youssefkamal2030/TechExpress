using System;
using System.Threading.Tasks;
using TechXpress.DataAccess.Data;

namespace TechXpress.DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IOrderRepository Orders { get; }
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IShoppingCartRepository ShoppingCarts { get; }
        Task<int> SaveChangesAsync();
        TechXpressDbContext GetContext();
    }
}