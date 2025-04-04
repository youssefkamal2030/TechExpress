using Microsoft.AspNetCore.Mvc;
using TechXpress.Models.Dto_s;
using TechXpress.Services.Interfaces;

namespace TechXpress.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(int? categoryId, string searchTerm, decimal? minPrice, decimal? maxPrice)
        {
            IEnumerable<ProductDTO> products = Enumerable.Empty<ProductDTO>();
            IEnumerable<CategoryDTO> allCategories = Enumerable.Empty<CategoryDTO>();

            try
            {
                // Fetch all categories for the filter
                allCategories = await _productService.GetAllCategoriesAsync();

                // Fetch products based on filters
                products = await _productService.SearchProductsAsync(searchTerm, categoryId, minPrice, maxPrice);
            }
            catch (KeyNotFoundException ex)
            {
                ViewBag.Error = ex.Message;
            }

            // Pass categories and current filter values to the view via ViewBag
            ViewBag.Categories = allCategories;
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(products);
        }
        [HttpGet]
        public async Task<IActionResult> GetFilteredProducts(int? categoryId)
        {
            IEnumerable<ProductDTO> products = Enumerable.Empty<ProductDTO>();

            try
            {
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
                // Return a partial view with an error message if needed
                return PartialView("_ProductGrid", Enumerable.Empty<ProductDTO>());
            }

            // Return the partial view with the filtered products
            return PartialView("_ProductGrid", products);
        }
    }
}