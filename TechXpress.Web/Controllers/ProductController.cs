using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Security.Claims;
using TechXpress.Models.Dto_s;
using TechXpress.Services.Interfaces;

namespace TechXpress.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;

        public ProductController(IProductService productService, IReviewService reviewService)
        {
            _productService = productService;
            _reviewService = reviewService;
        }

        // GET: /Product/
        public async Task<IActionResult> Index(int? categoryId, string searchTerm, decimal? minPrice, decimal? maxPrice)
        {
            IEnumerable<ProductDTO> products = Enumerable.Empty<ProductDTO>();
            IEnumerable<CategoryDTO> allCategories = Enumerable.Empty<CategoryDTO>();

            try
            {
                allCategories = await _productService.GetAllCategoriesAsync();

                // Start with all products or category filtered products
                if (categoryId.HasValue)
                {
                    products = await _productService.GetProductsByCategoryAsync(categoryId.Value);
                }
                else
                {
                    products = await _productService.GetAllProductsAsync();
                }

                // Apply search term filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    products = products.Where(p => 
                        p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                        (p.Description != null && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
                }

                // Apply price filters
                if (minPrice.HasValue)
                {
                    products = products.Where(p => p.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    products = products.Where(p => p.Price <= maxPrice.Value);
                }
            }
            catch (KeyNotFoundException ex)
            {
                ViewBag.Error = ex.Message;
            }

            // Pass all filters to view for displaying current selections
            ViewBag.Categories = allCategories;
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View("Index", products);
        }

        // GET: /Product/GetFilteredProducts - For AJAX calls
        [HttpGet]
        public async Task<IActionResult> GetFilteredProducts(int? categoryId, string searchTerm, decimal? minPrice, decimal? maxPrice)
        {
            IEnumerable<ProductDTO> products = Enumerable.Empty<ProductDTO>();

            try
            {
                // Start with all products or category filtered products
                if (categoryId.HasValue)
                {
                    products = await _productService.GetProductsByCategoryAsync(categoryId.Value);
                }
                else
                {
                    products = await _productService.GetAllProductsAsync();
                }

                // Apply search term filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    products = products.Where(p => 
                        p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                        (p.Description != null && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
                }

                // Apply price filters
                if (minPrice.HasValue)
                {
                    products = products.Where(p => p.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    products = products.Where(p => p.Price <= maxPrice.Value);
                }
            }
            catch (KeyNotFoundException ex)
            {
                return PartialView("_ProductGrid", Enumerable.Empty<ProductDTO>());
            }

            return PartialView("_ProductGrid", products);
        }

        // GET: /Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
                
                var product = await _productService.GetProductWithReviewsAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                ViewBag.UserHasReviewed = userId != null && await _reviewService.HasUserReviewedProductAsync(userId, id);
                
                ViewBag.NewReview = new ReviewDTO { ProductId = id };

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
