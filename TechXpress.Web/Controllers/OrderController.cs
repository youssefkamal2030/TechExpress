using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechXpress.Application.Services;
using TechXpress.Application.Interfaces;

namespace TechXpress.Web.Controllers
{
    [Authorize] 
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(OrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

      
        public async Task<IActionResult> Checkout()
        {
            var cartItems = await _cartService.GetCartItemsAsync();
            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }
            return View(cartItems);
        }

       
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string stripeToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = (await _cartService.GetCartItemsAsync()).ToList();
            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            try
            {
                await _orderService.PlaceOrderAsync(userId, cartItems);
                await _cartService.ClearCartAsync();
                return RedirectToAction("OrderConfirmation");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Order placement failed: " + ex.Message);
                return View("Checkout", cartItems);
            }
        }

       
        public IActionResult OrderConfirmation()
        {
            return View();
        }
    }
}