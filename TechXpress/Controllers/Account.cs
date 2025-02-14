using Azure.Core.Pipeline;
using Microsoft.AspNetCore.Mvc;
using TechXpress.Repositories;
using TechXpress.Models;

namespace TechXpress.Controllers
{
  
    public class Account : Controller
    {
        private readonly UnitOfWork _worker;
        
        public Account(UnitOfWork worker)
        {
            _worker = worker;
        }
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SingIn(Users user)
        {
            
            return View();

        }
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(Users user) 
            {
            var existinguser = _worker.Users.GetByEmailAsync(user.Email);
            if(existinguser != null)
            {
                
            }
                _worker.Users.Create(user, user.Password);
                return View("SignIn");
            }
    }
}
