using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TechXpress.Models;
using TechXpress.Repositories;

namespace TechXpress.Controllers
{
    

    public class HomeController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, UnitOfWork worker)

        { 
            _logger = logger;
            _unitOfWork = worker;

        }

        public IActionResult Index()
        {
            var categories = _unitOfWork.Categories.GetAll() ?? new List<Categories>();
            return View(categories);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
