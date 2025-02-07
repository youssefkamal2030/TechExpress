using Azure.Core.Pipeline;
using Microsoft.AspNetCore.Mvc;
using TechXpress.Repositories;

namespace TechXpress.Controllers
{
  
    public class Account : Controller
    {
        private readonly UsersRepo usersRepo;
        public Account(UsersRepo usersRepo)
        {
            this.usersRepo = usersRepo;
        }
        public IActionResult SignIn()
        {
            var newUser = usersRepo.CreateUser()
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }
    }
}
