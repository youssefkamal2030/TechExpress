using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechXpress.DataAccess.Interfaces;
using TechXpress.Models.Dto_s;
using TechXpress.Models.entitis;
using TechXpress.Services.Interfaces;

namespace TechXpress.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            if(products == null)
            {
                throw new KeyNotFoundException("No products found.");
            }
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if(product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _unitOfWork.Products.GetAllAsync();
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
    }
}