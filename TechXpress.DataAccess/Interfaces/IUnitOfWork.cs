namespace TechXpress.DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        TechXpress.DataAccess.Data.TechXpressDbContext _context { get; }
        IOrderRepository Orders { get; }
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IShoppingCartRepository ShoppingCarts { get; }
        Task<int> SaveChangesAsync();
    }
}