using Microsoft.AspNetCore.Mvc;
using TechXpress.Application.Services;
using TechXpress.Models.Dto_s;
using TechXpress.Models.entitis;
namespace TechXpress.Web.Controllers
{
    public class ProductController : Controller
    {
            private readonly ProductService _productService;

            public ProductController(ProductService productService)
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

            return View("Index",products);
        }

        // GET: /Product/Details/5
        public async Task<IActionResult> Details(int id)
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return View(product);
            }
       
    }

}
