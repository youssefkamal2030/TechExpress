using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechXpress.Models.entitis;
using TechXpress.Services.Interfaces;

namespace TechXpress.Web.Controllers
{
    [Authorize] 
    public class CartController : Controller
    {
        private readonly IShoppingCartService _cartService;

        public CartController(IShoppingCartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            return View(cart);
        }

        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.AddToCartAsync(id, quantity, userId);
            return RedirectToAction("Index", "Home"); 
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.RemoveFromCartAsync(productId, userId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
           
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.ClearCartAsync(userId);
            return RedirectToAction("Index");
        }
    }
}