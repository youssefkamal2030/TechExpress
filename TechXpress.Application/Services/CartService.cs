using TechXpress.Application.Interfaces;
using TechXpress.Domain.Entities;

namespace TechXpress.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartStorage _cartStorage;
        private readonly IUnitOfWork _unitOfWork;

        public CartService(ICartStorage cartStorage, IUnitOfWork unitOfWork)
        {
            _cartStorage = cartStorage;
            _unitOfWork = unitOfWork;
        }

        public async Task AddToCartAsync(int productId, int quantity)
        {
            var cart = await _cartStorage.GetCartAsync() ?? new List<CartItem>();
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) throw new Exception("Product not found");

            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity
                });
            }
            await _cartStorage.SaveCartAsync(cart);
        }

        public async Task RemoveFromCartAsync(int productId)
        {
            var cart = await _cartStorage.GetCartAsync();
            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.ProductId == productId);
                if (item != null)
                {
                    cart.Remove(item);
                    await _cartStorage.SaveCartAsync(cart);
                }
            }
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync()
        {
            return await _cartStorage.GetCartAsync() ?? new List<CartItem>();
        }

        public async Task ClearCartAsync()
        {
            await _cartStorage.ClearCartAsync();
        }
    }
}