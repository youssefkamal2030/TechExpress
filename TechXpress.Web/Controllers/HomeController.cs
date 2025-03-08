using Microsoft.AspNetCore.Mvc;
using TechXpress.Infrastructure.Data;

namespace TechXpress.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public HomeController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
