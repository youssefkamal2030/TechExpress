//using TechXpress.DataAccess.Interfaces;
//using TechXpress.Models.entitis;
//using TechXpress.Services.Interfaces;

//namespace TechXpress.Services.Services
//{
//    public class OrderService : IOrderService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IPaymentGateway _paymentGateway;

//        public OrderService(IUnitOfWork unitOfWork, IPaymentGateway paymentGateway)
//        {
//            _unitOfWork = unitOfWork;
//            _paymentGateway = paymentGateway;
//        }

//        public Task<bool> CreateOrderAsync(Order order)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<bool> DeleteOrderAsync(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<Order>> GetAllOrdersAsync()
//        {
//            throw new NotImplementedException();
//        }

//        public Task<Order> GetOrderByIdAsync(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task PlaceOrderAsync(string userId, List<CartItem> cartItems)
//        {
//            var total = cartItems.Sum(item => item.Price * item.Quantity);
//            var order = new Order
//            {
//                UserId = userId,
//                OrderDate = DateTime.UtcNow,
//                Status = "Pending",
//                TotalAmount = total,
//                OrderItems = cartItems.Select(item => new OrderItem
//                {
//                    ProductId = item.ProductId,
//                    Quantity = item.Quantity,
//                    UnitPrice = item.Price
//                }).ToList()
//            };

//            await _unitOfWork.Orders.AddAsync(order);
//            await _unitOfWork.SaveChangesAsync();

//            // Process payment (simplified)
//            var paymentSuccess = await _paymentGateway.ProcessPaymentAsync(total, "stripe_token");
//            if (paymentSuccess)
//            {
//                order.Status = "Paid";
//                foreach (var item in order.OrderItems)
//                {
//                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
//                    product.Stock -= item.Quantity;
//                    await _unitOfWork.Products.UpdateAsync(product);
//                }
//                await _unitOfWork.SaveChangesAsync();
//            }
//            else
//            {
//                order.Status = "Failed";
//                await _unitOfWork.SaveChangesAsync();
//            }
//        }

//        public Task<bool> UpdateOrderAsync(Order order)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}