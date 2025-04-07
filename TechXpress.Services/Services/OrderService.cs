using AutoMapper;
using TechXpress.DataAccess.Interfaces;
using TechXpress.Models.Dto_s;
using TechXpress.Models.entitis;
using TechXpress.Services.Interfaces;

namespace TechXpress.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IPaymentGateway paymentGateway, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
            _mapper = mapper;
        }

        public async Task<bool> CreateOrderAsync(Order order)
        {
            try
            {
                if (order == null)
                    throw new ArgumentNullException(nameof(order));

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                return false;
            }
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            try
            {
                if (order == null)
                    throw new ArgumentNullException(nameof(order));

                var existingOrder = await _unitOfWork.Orders.GetByIdAsync(order.Id);
                if (existingOrder == null)
                    throw new KeyNotFoundException($"Order with ID {order.Id} not found");

                _mapper.Map(order, existingOrder);
                await _unitOfWork.Orders.UpdateAsync(existingOrder);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                return false;
            }
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(id);
                if (order == null)
                    throw new KeyNotFoundException($"Order with ID {id} not found");

                await _unitOfWork.Orders.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                return false;
            }
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(id);
                if (order == null)
                    throw new KeyNotFoundException($"Order with ID {id} not found");

                return order;
            }
            catch (Exception ex)
            {
                // Log the error
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAllAsync();
                if (orders == null || !orders.Any())
                    throw new KeyNotFoundException("No orders found");

                return orders;
            }
            catch (Exception ex)
            {
                // Log the error
                throw;
            }
        }

        public async Task<Order> PlaceOrderAsync(string userId, List<CartItem> cartItems, string paymentToken)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");
            if (cartItems == null || !cartItems.Any())
                throw new ArgumentException("Cart items cannot be empty", nameof(cartItems));
            
            // Calculate total
            decimal total = cartItems.Sum(item => (decimal)item.PriceAtAdd * item.Quantity);
            if (total <= 0)
                throw new InvalidOperationException("Total amount must be greater than zero");
            
            // Create the order
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
                    UnitPrice = (decimal)item.PriceAtAdd
                }).ToList()
            };

            // Debug logging
            Console.WriteLine($"Creating order for user {userId} with {order.OrderItems.Count} items");
            Console.WriteLine($"Payment token: {paymentToken}");
            Console.WriteLine($"Total amount: {total}");

            try
            {
                // Start a transaction to ensure all operations succeed or fail together
                using (var transaction = _unitOfWork._context.Database.BeginTransaction())
                {
                    try
                    {
                        // For Stripe Checkout, payment is already processed, so we assume success
                        // if we have a PaymentIntent ID
                        bool paymentSuccess = !string.IsNullOrEmpty(paymentToken);
                        
                        if (paymentSuccess)
                        {
                            // Update order status
                            order.Status = "Paid";
                            
                            // Update product stock
                            foreach (var item in order.OrderItems)
                            {
                                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                                if (product == null)
                                {
                                    Console.WriteLine($"Product with ID {item.ProductId} not found");
                                    throw new InvalidOperationException($"Product with ID {item.ProductId} not found");
                                }
                                
                                if (product.Stock < item.Quantity)
                                {
                                    Console.WriteLine($"Insufficient stock for product {product.Name}: {product.Stock} available, {item.Quantity} requested");
                                    throw new InvalidOperationException($"Insufficient stock for product {product.Name}");
                                }

                                product.Stock -= item.Quantity;
                                await _unitOfWork.Products.UpdateAsync(product);
                            }

                            // Save the order with its items
                            await _unitOfWork.Orders.AddAsync(order);
                            await _unitOfWork.SaveChangesAsync();
                            Console.WriteLine($"Order saved with ID: {order.Id}");

                            // Clear the cart
                            var cart = await _unitOfWork.ShoppingCarts.GetCartByUserIdAsync(userId);
                            if (cart != null)
                            {
                                cart.Items.Clear();
                                await _unitOfWork.ShoppingCarts.UpdateAsync(cart);
                                await _unitOfWork.SaveChangesAsync();
                                Console.WriteLine("Cart cleared successfully");
                            }
                            
                            // Commit the transaction
                            await transaction.CommitAsync();
                            Console.WriteLine("Transaction committed successfully");
                        }
                        else
                        {
                            Console.WriteLine("Payment failed: No payment token provided");
                            order.Status = "Payment Failed";
                            await _unitOfWork.Orders.AddAsync(order);
                            await _unitOfWork.SaveChangesAsync();
                        }

                        return order;
                    }
                    catch (Exception ex)
                    {
                        // Roll back the transaction on error
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Transaction rolled back due to error: {ex.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error with details
                Console.WriteLine($"Error in PlaceOrderAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                
                // Try to save the order in error state for auditing purposes
                order.Status = "Error";
                try
                {
                    await _unitOfWork.Orders.AddAsync(order);
                    await _unitOfWork.SaveChangesAsync();
                    Console.WriteLine($"Order saved in error state with ID: {order.Id}");
                }
                catch (Exception saveEx)
                {
                    // If we can't even save the error state
                    Console.WriteLine($"Failed to save error state: {saveEx.Message}");
                }
                
                throw new InvalidOperationException($"Failed to process order: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByUserIdAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new ArgumentNullException(nameof(userId));

                var orders = await _unitOfWork.Orders.GetAllAsync();
                var userOrders = orders.Where(o => o.UserId == userId).ToList();
                
                if (!userOrders.Any())
                    return new List<OrderDTO>();

                return _mapper.Map<IEnumerable<OrderDTO>>(userOrders);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in GetOrdersByUserIdAsync: {ex.Message}");
                throw;
            }
        }
    }
}