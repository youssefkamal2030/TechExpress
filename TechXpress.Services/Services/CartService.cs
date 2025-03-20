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
        public CartService(IUnitOfWork unitOfWork , IMapper mapper)
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

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new ArgumentException("Product not found.", nameof(productId));
            }

           
            await _unitOfWork.ShoppingCarts.AddItemToCartAsync(userId, productId, quantity);
        }

        public async Task RemoveFromCartAsync(int productId, string userId)
        {
          
            await _unitOfWork.ShoppingCarts.RemoveItemFromCartAsync(userId, productId);
        }

        public async Task ClearCartAsync(string userId)
        {
            
            await _unitOfWork.ShoppingCarts.ClearCartAsync(userId);
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
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    total += product.Price * item.Quantity;
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