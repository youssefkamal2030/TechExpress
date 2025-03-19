//using TechXpress.DataAccess.Interfaces;
//using TechXpress.Models.entitis;
//using TechXpress.Services.Interfaces;

//namespace TechXpress.Services.Services
//{
//    public class CartService : IShoppingCartService
//    {
//        private readonly IUnitOfWork _unitOfWork;

//        public CartService(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }

//        public async Task AddToCartAsync(int productId, int quantity, string userId)
//        {
//            var cartItem = await _unitOfWork.ShoppingCarts.GetByUserIdAndProductIdAsync(userId, productId);
//            if (cartItem == null)
//            {
//                cartItem = new ShoppingCart { UserId = userId, ProductId = productId, Quantity = quantity };
//                await _unitOfWork.ShoppingCarts.AddAsync(cartItem);
//            }
//            else
//            {
//                cartItem.Quantity += quantity;
//                await _unitOfWork.ShoppingCarts.UpdateAsync(cartItem);
//            }
//            await _unitOfWork.SaveChangesAsync();
//        }

//        public async Task RemoveFromCartAsync(int productId, string userId)
//        {
//            var cartItem = await _unitOfWork.ShoppingCarts.GetByUserIdAndProductIdAsync(userId, productId);
//            if (cartItem != null)
//            {
//                await _unitOfWork.ShoppingCarts.DeleteAsync(cartItem.Id);
//                await _unitOfWork.SaveChangesAsync();
//            }
//        }

//        public async Task ClearCartAsync(string userId)
//        {
//            var cartItems = await _unitOfWork.ShoppingCarts.GetByUserIdAsync(userId);
//            foreach (var item in cartItems)
//            {
//                await _unitOfWork.ShoppingCarts.DeleteAsync(item.Id);
//            }
//            await _unitOfWork.SaveChangesAsync();
//        }

//        public async Task<int> GetCartCountAsync(string userId)
//        {
//            var cartItems = await _unitOfWork.ShoppingCarts.GetByUserIdAsync(userId);
//            return cartItems.Count();
//        }

//        public async Task<decimal> GetCartTotalAsync(string userId)
//        {
//            var cartItems = await _unitOfWork.ShoppingCarts.GetByUserIdAsync(userId);
//            decimal total = 0;
//            foreach (var item in cartItems)
//            {
//                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
//                total += product.Price * item.Quantity;
//            }
//            return total;
//        }
//    }
//}