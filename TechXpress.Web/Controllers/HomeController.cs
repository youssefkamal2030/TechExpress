using Microsoft.AspNetCore.Mvc;
using TechXpress.Application.Dto_s;
using TechXpress.Application.Interfaces;
using TechXpress.Infrastructure.Data;

namespace TechXpress.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl ?? string.Empty,
                CreatedAt = p.CreatedAt,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name 
            }).ToList();
            return View(productDtos);
        }
    }
}
