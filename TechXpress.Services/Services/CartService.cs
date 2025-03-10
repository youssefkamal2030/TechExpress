    //using TechXpress.DataAccess.Interfaces;
    //using TechXpress.Models.entitis;
    //namespace TechXpress.Application.Services
    //{
    //    public class CartService
    //    {

    //        private readonly IUnitOfWork _unitOfWork;
    //        public CartService(IUnitOfWork unitOfWork)
    //        {
    //            _unitOfWork = unitOfWork;
    //        }
    //        public async Task AddToCartAsync(string userId, int productId, int quantity)
    //        {
    //            var cartItem = await _unitOfWork.ShoppingCarts.GetByUserIdAndProductIdAsync(userId, productId);
    //        if (cartItem == null)
    //            {
    //                cartItem = new ShoppingCart
    //                {
    //                    UserId = userId,
    //                    ProductId = productId,
    //                    Quantity = quantity
    //                };
    //                await _unitOfWork.ShoppingCarts.AddAsync(cartItem);
    //            }
    //            else
    //            {
    //                cartItem.Quantity += quantity;
    //                await _unitOfWork.ShoppingCarts.UpdateAsync(cartItem);
    //            }
    //            await _unitOfWork.SaveChangesAsync();
    //        }
    //        public async Task RemoveFromCartAsync(string userId, int productId)
    //        {
    //            var cartItem = await _unitOfWork.ShoppingCarts.GetByUserIdAndProductIdAsync(userId, productId);
    //            if (cartItem != null)
    //            {
    //                await _unitOfWork.ShoppingCarts.DeleteAsync(cartItem.Id);
    //                await _unitOfWork.SaveChangesAsync();
    //            }
    //        }
    //        public async Task<IEnumerable<ShoppingCart>> GetCartItemsAsync(string userId)
    //        {
    //            return await _unitOfWork.ShoppingCarts.GetByUserIdAsync(userId);
    //        }

    //    }
    //}