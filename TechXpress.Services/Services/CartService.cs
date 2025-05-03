using AutoMapper;
using TechXpress.DataAccess.Interfaces;
using TechXpress.Models.Dto_s;
using TechXpress.Models.entitis;
using TechXpress.Services.Interfaces;

namespace TechXpress.Services.Services
{
    public class CartService : IShoppingCartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddToCartAsync(int productId, int quantity, string userId)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            }

            var result = await _unitOfWork.ShoppingCarts.AddItemToCartAsync(userId, productId, quantity);
            if (!result)
            {
                throw new InvalidOperationException("Failed to add item to cart.");
            }
        }

        public async Task RemoveFromCartAsync(int productId, string userId)
        {
            var result = await _unitOfWork.ShoppingCarts.RemoveItemFromCartAsync(userId, productId);
            if (!result)
            {
                throw new InvalidOperationException("Failed to remove item from cart.");
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            var result = await _unitOfWork.ShoppingCarts.ClearCartAsync(userId);
            if (!result)
            {
                throw new InvalidOperationException("Failed to clear cart.");
            }
        }

        public async Task<int> GetCartCountAsync(string userId)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any())
            {
                return 0;
            }
            return cart.Items.Sum(item => item.Quantity);
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any())
            {
                return 0;
            }

            decimal total = 0;
            foreach (var item in cart.Items)
            {
                if (item.PriceAtAdd.HasValue)
                {
                    total += item.PriceAtAdd.Value * item.Quantity;
                }
                else
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        total += product.Price * item.Quantity;
                    }
                }
            }
            return total;
        }

        public async Task<ShoppingCartDTO> GetCartByUserIdAsync(string userId)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetCartByUserIdAsync(userId);
            return _mapper.Map<ShoppingCartDTO>(cart);
        }
    }
}