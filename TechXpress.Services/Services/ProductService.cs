using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechXpress.DataAccess.Interfaces;
using TechXpress.Models.Dto_s;
using TechXpress.Models.entitis;
using TechXpress.Services.Interfaces;

namespace TechXpress.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IReviewService _reviewService;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IReviewService reviewService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
        }

        public async Task<ProductDTO> GetProductWithReviewsAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdWithCategoryAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }

            var productDto = _mapper.Map<ProductDTO>(product);
            
            // Add reviews
            var reviews = await _reviewService.GetProductReviewsAsync(id);
            productDto.Reviews = reviews.ToList();
            productDto.ReviewCount = reviews.Count();
            
            // Add rating statistics
            productDto.AverageRating = await _reviewService.GetAverageRatingAsync(id);
            productDto.RatingBreakdown = await _reviewService.GetRatingBreakdownAsync(id);
            
            return productDto;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllWithCategoryAsync();
            if(products == null)
            {
                throw new KeyNotFoundException("No products found.");
            }
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetPaginatedProductsAsync(int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;
            var products = await _unitOfWork.Products.GetPaginatedProductsAsync(skip, pageSize);
            if(products == null)
            {
                throw new KeyNotFoundException("No products found.");
            }
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<int> GetProductCountAsync()
        {
            return await _unitOfWork.Products.GetProductCountAsync();
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdWithCategoryAsync(id);
            if(product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _unitOfWork.Products.GetAllWithCategoryAsync();
            var filteredProducts = products.Where(p => p.CategoryId == categoryId);
            return _mapper.Map<IEnumerable<ProductDTO>>(filteredProducts);
        }

        public async Task AddProductAsync(ProductDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(ProductDTO productDto)
        {
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(productDto.Id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {productDto.Id} not found.");
            }

            // If CategoryId is provided, ensure the Category object is loaded
            if (productDto.CategoryId > 0 && existingProduct.Category?.Id != productDto.CategoryId)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(productDto.CategoryId);
                if (category != null)
                {
                    existingProduct.Category = category;
                }
            }

            _mapper.Map(productDto, existingProduct); 
            await _unitOfWork.Products.UpdateAsync(existingProduct);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }

            await _unitOfWork.Products.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            if(categories == null)
            {
                throw new KeyNotFoundException("No categories found.");
            }
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<IEnumerable<ProductDTO>> SearchProductsAsync(string searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var products = await _unitOfWork.Products.GetAllWithCategoryAsync();
            
            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                products = products.Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                             p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByCategoryIdAsync(int categoryId)
        {
            var products = await _unitOfWork.Products.FindAsync(p => p.CategoryId == categoryId);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        // Category methods
        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task AddCategoryAsync(CategoryDTO categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(CategoryDTO categoryDto)
        {
            var existingCategory = await _unitOfWork.Categories.GetByIdAsync(categoryDto.Id);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Category with ID {categoryDto.Id} not found.");
            }

            _mapper.Map(categoryDto, existingCategory);
            await _unitOfWork.Categories.UpdateAsync(existingCategory);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            // Check if any products are using this category
            var productsInCategory = await _unitOfWork.Products.FindAsync(p => p.CategoryId == id);
            if (productsInCategory.Any())
            {
                throw new InvalidOperationException("Cannot delete category because it contains products. Remove or reassign the products first.");
            }

            await _unitOfWork.Categories.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}