﻿using Microsoft.AspNetCore.Mvc;
using TechXpress.Application.Interfaces;
using TechXpress.Domain.Entities;

namespace TechXpress.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

      
        public async Task<IActionResult> Index()
        {
            var cartItems = await _cartService.GetCartItemsAsync();
            return View(cartItems);
        }

     
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            await _cartService.AddToCartAsync(productId, quantity);
            return RedirectToAction("Index");
        }

     
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            await _cartService.RemoveFromCartAsync(productId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            await _cartService.ClearCartAsync();
            return RedirectToAction("Index");
        }
    }
}