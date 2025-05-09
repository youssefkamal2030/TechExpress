using TechXpress.Models.entitis;

namespace TechXpress.DataAccess.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllWithCategoryAsync();
        Task<Product> GetByIdWithCategoryAsync(int id);
        Task<IEnumerable<Product>> GetPaginatedProductsAsync(int skip, int take);
        Task<int> GetProductCountAsync();
    }
}