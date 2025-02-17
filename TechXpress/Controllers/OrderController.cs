using Microsoft.AspNetCore.Mvc;
using TechXpress.Models;
using TechXpress.Repositories;
using System.Text.Json;

namespace TechXpress.Controllers
{
    public class OrderController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private const string CartSessionKey = "Cart";

        public OrderController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }
            return View(cart);
        }

        [HttpPost]
        public IActionResult ConfirmCheckout()
        {
            var cart = GetCart();
            foreach (var item in cart)
            {
                var orderDetail = new OrderDetails
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                
                };
                _unitOfWork.OrdersDetails.Create(orderDetail);
            }

            ClearCart();
            return RedirectToAction("Confirmation");
        }

        public IActionResult Confirmation()
        {
            return View();
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return cartJson == null ? new List<CartItem>() :
                JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }

        private void ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
        }
    }
}