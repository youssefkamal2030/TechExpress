using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using TechXpress.Models.Dto_s;
using TechXpress.Services.Interfaces;

namespace TechXpress.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: /Product/
        public async Task<IActionResult> Index(int? categoryId)
        {
            IEnumerable<ProductDTO> products = Enumerable.Empty<ProductDTO>();
            IEnumerable<CategoryDTO> allCategories = Enumerable.Empty<CategoryDTO>();

            try
            {
                allCategories = await _productService.GetAllCategoriesAsync();

                if (categoryId.HasValue)
                {
                    products = await _productService.GetProductsByCategoryAsync(categoryId.Value);
                }
                else
                {
                    products = await _productService.GetAllProductsAsync();
                }
            }
            catch (KeyNotFoundException ex)
            {
                ViewBag.Error = ex.Message;
            }

            ViewBag.Categories = allCategories;
            ViewBag.CurrentCategoryId = categoryId;

            return View("Index", products);
        }

        // GET: /Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Get related products (products from the same category)
                var relatedProducts = await _productService.GetProductsByCategoryAsync(product.CategoryId);
                relatedProducts = relatedProducts.Where(p => p.Id != product.Id).Take(4);

                ViewBag.RelatedProducts = relatedProducts;
                return View(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }

    }

}
