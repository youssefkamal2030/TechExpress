using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Models.Dto_s;
using TechXpress.Models.entitis;
namespace TechXpress.Services.Interfaces
{
    public interface IProductService
    {
       
            Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
            Task<IEnumerable<ProductDTO>> GetPaginatedProductsAsync(int page, int pageSize);
            Task<int> GetProductCountAsync();
            Task<ProductDTO> GetProductByIdAsync(int id);
            Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId);
            Task<IEnumerable<ProductDTO>> GetProductsByCategoryIdAsync(int categoryId);
            Task AddProductAsync(ProductDTO productDto); 
            Task UpdateProductAsync(ProductDTO productDto); 
            Task DeleteProductAsync(int id);
            Task<ProductDTO> GetProductWithReviewsAsync(int id);
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<IEnumerable<ProductDTO>> SearchProductsAsync(string searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice);
        Task<CategoryDTO> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(CategoryDTO categoryDto);
        Task UpdateCategoryAsync(CategoryDTO categoryDto);
        Task DeleteCategoryAsync(int id);
    }
}
