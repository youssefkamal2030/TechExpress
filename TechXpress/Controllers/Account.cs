using Azure.Core.Pipeline;
using Microsoft.AspNetCore.Mvc;
using TechXpress.Repositories;
using TechXpress.Models;
using Microsoft.AspNetCore.Identity;

namespace TechXpress.Controllers
{
  
    public class Account : Controller
    {
        private readonly UnitOfWork _worker;
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        public Account(UnitOfWork worker, UserManager<Users> usermanager, SignInManager<Users> signInManager)
        {
            _worker = worker;
            _userManager = usermanager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(Users user)
        {
            if (ModelState.IsValid)
            {
                var ExistingUser = await _userManager.FindByEmailAsync(user.Email);
                
                if (ExistingUser == null)
                {
                    ModelState.AddModelError(nameof(user.UserName), "user not found");
                }
                else
                {
                    var result =await _signInManager.CheckPasswordSignInAsync(ExistingUser, user.Password, false);
                    if (result.Succeeded)
                    {
                        var cookieOptions = new CookieOptions
                        {
                            Secure = true,
                            Expires = DateTime.Now.AddDays(2)
                        };
                        Response.Cookies.Append("username",ExistingUser.UserName, cookieOptions);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Password");
                    }
                }
            }
            return View(user);
        }
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(Users user)
        {
            var ExisitngUsername = await _userManager.FindByNameAsync(user.UserName);
            if(ExisitngUsername != null)
            {
                ModelState.AddModelError(nameof(user.UserName), "username Already Exists");
            }
            var ExistingUserEmail = await _userManager.FindByEmailAsync(user.Email);
            if(ExistingUserEmail != null)
            {
                ModelState.AddModelError(nameof(user.Email), "Email Already Exists");
            }
           if(ModelState.IsValid)
            {
                var result =await _userManager.CreateAsync(user,user.Password);
                if(result.Succeeded)
                {
                    return RedirectToAction(nameof(SignIn));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                    

            }
            return View(user);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("username");

            return RedirectToAction("Index", "Home");
        }


    }
}
