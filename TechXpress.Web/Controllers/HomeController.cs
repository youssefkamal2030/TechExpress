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

        public async Task<IActionResult> Index()
        {
            var products = new List<ProductDTO>();
            try
            {
                products = (await _productService.GetAllProductsAsync()).ToList();
            }
            catch (KeyNotFoundException ex)
            {
                ViewBag.Error = ex.Message;

            }
            return View(products);

        }
    }
}