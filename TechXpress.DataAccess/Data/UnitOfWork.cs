using System.Threading.Tasks;
using TechXpress.DataAccess.Interfaces;
using TechXpress.DataAccess.Repositories;
using TechXpress.Models.entitis;

namespace TechXpress.DataAccess.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TechXpressDbContext _context;
        public IOrderRepository Orders { get; private set; }
        public IProductRepository Products { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public IShoppingCartRepository ShoppingCarts { get; private set; }

        public UnitOfWork(TechXpressDbContext context)
        {
            _context = context;
            Orders = new OrderRepository(_context);
            Products = new ProductRepository(_context);
            Categories = new CategoryRepository(_context);
            ShoppingCarts = new ShoppingCartRepository(_context);
        }

        public TechXpressDbContext GetContext()
        {
            return _context;
        }
       
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