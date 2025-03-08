using Microsoft.AspNetCore.Http;
using System.Text.Json;
using TechXpress.Application.Interfaces;
using TechXpress.Domain.Entities;

namespace TechXpress.Infrastructure.Services
{
    public class SessionCartStorage : ICartStorage
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartKey = "Cart";

        public SessionCartStorage(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SaveCartAsync(List<CartItem> cartItems)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString(CartKey, JsonSerializer.Serialize(cartItems));
        }

        public async Task<List<CartItem>> GetCartAsync()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = session.GetString(CartKey);
            return cartJson != null ? JsonSerializer.Deserialize<List<CartItem>>(cartJson) : new List<CartItem>();
        }

        public async Task ClearCartAsync()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.Remove(CartKey);
        }
    }
}