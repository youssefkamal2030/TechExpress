using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TechXpress.Models.Dto_s;
using TechXpress.Services.Interfaces;

namespace TechXpress.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IMemoryCache _memoryCache;
        private const int PageSize = 4;
        private const string CategoriesCacheKey = "AllCategories";

        public HomeController(IProductService productService, IMemoryCache memoryCache)
        {
            _productService = productService;
            _memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            IEnumerable<ProductDTO> products = Enumerable.Empty<ProductDTO>();
            IEnumerable<CategoryDTO> allCategories = Enumerable.Empty<CategoryDTO>();

            try
            {
                // Get categories with caching
                if (!_memoryCache.TryGetValue(CategoriesCacheKey, out allCategories))
                {
                    allCategories = await _productService.GetAllCategoriesAsync();
                    
                    // Calculate product counts for categories (this is still an N+1 query but now cached)
                    foreach (var category in allCategories)
                    {
                        var productsInCategory = await _productService.GetProductsByCategoryAsync(category.Id);
                        category.ProductCount = productsInCategory.Count();
                    }
                    
                    // Cache categories for 10 minutes
                    _memoryCache.Set(CategoriesCacheKey, allCategories, TimeSpan.FromMinutes(10));
                }

                // Get paginated products using the database-optimized methods
                products = await _productService.GetPaginatedProductsAsync(page, PageSize);
                
                // Check if there are more products
                int totalProducts = await _productService.GetProductCountAsync();
                
                ViewBag.CurrentPage = page;
                ViewBag.HasMoreProducts = totalProducts > page * PageSize;
            }
            catch (KeyNotFoundException ex)
            {
                ViewBag.Error = ex.Message;
            }

            ViewBag.Categories = allCategories;

            return View(products);
        }
        
        [HttpGet]
        public async Task<IActionResult> LoadMoreProducts(int page = 2)
        {
            try
            {
                var products = await _productService.GetPaginatedProductsAsync(page, PageSize);
                int totalProducts = await _productService.GetProductCountAsync();
                
                ViewBag.CurrentPage = page;
                ViewBag.HasMoreProducts = totalProducts > page * PageSize;
                
                return PartialView("_ProductGrid", products);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Subscribe(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Please provide a valid email address.";
                return RedirectToAction("Index");
            }
            TempData["SuccessMessage"] = "Thank you for subscribing to our newsletter!";
            return RedirectToAction("Index");
        }
    }
}