using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechXpress.Domain.Entities;
using TechXpress.Infrastructure.Data;
namespace TechXpress.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UnitOfWork _Worker;
        private readonly UserManager<User> _UserManager;
        public AccountController(UnitOfWork worker, UserManager<User> userManager) {
            _Worker = worker;
            _UserManager = userManager;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "admin")
            {
                HttpContext.Session.SetString("username", username);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                _UserManager.CreateAsync(user, user.password);

                return RedirectToAction("Login");
            }
            return View();
        }
    }
}
