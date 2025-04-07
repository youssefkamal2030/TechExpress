using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Climate;
using System.Security.Claims;
using TechXpress.Services.Services;
using TechXpress.Models.entitis;
using TechXpress.Services.Interfaces;

namespace TechXpress.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}