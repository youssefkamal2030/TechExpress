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
            Task<ProductDTO> GetProductByIdAsync(int id);
            Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId);
            Task AddProductAsync(ProductDTO productDto); 
            Task UpdateProductAsync(ProductDTO productDto); 
            Task DeleteProductAsync(int id);
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<IEnumerable<ProductDTO>> SearchProductsAsync(string searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice);
    }
}
