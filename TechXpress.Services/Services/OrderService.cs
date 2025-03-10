using TechXpress.DataAccess.Interfaces;
using TechXpress.Models.entitis;

namespace TechXpress.Application.Services
{
    public class OrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;

        public OrderService(IUnitOfWork unitOfWork, IPaymentGateway paymentGateway)
        {
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
        }

        public async Task PlaceOrderAsync(string userId, List<CartItem> cartItems)
        {
            var total = cartItems.Sum(item => item.Price * item.Quantity);
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = total,
                OrderItems = cartItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                }).ToList()
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Process payment (simplified)
            var paymentSuccess = await _paymentGateway.ProcessPaymentAsync(total, "stripe_token");
            if (paymentSuccess)
            {
                order.Status = "Paid";
                foreach (var item in order.OrderItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    product.Stock -= item.Quantity;
                    await _unitOfWork.Products.UpdateAsync(product);
                }
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                order.Status = "Failed";
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}