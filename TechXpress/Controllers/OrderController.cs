using Microsoft.AspNetCore.Mvc;

namespace TechXpress.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
