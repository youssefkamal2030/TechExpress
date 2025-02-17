using Microsoft.AspNetCore.Mvc;
using TechXpress.Models;
using TechXpress.Repositories;
using System.Text.Json;

namespace TechXpress.Controllers
{
    public class CartController : Controller
    {
        private readonly  UnitOfWork _unitOfWork;
        private const string CartSessionKey = "Cart";

        public CartController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(Products NewProduct, int quantity)
        {
            var product = _unitOfWork.Products.GetByID(NewProduct.ProductID);
            if (product == null)
            {
                return NotFound();
            }

            var cart = GetCart();

            var existingItem = cart.FirstOrDefault(item => item.ProductId == product.ProductID);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.ProductID,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Quantity = quantity
                });
            }

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var itemToRemove = cart.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return cartJson == null ? new List<CartItem>() :
                JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }

        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }
    }

}