using Microsoft.AspNetCore.Mvc;
using TechXpress.Application.Services;
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
            public async Task<IActionResult> Index()
            {
                var products = await _productService.GetAllProductsAsync();
                return View(products);
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
