using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<bool> CreateOrderAsync(OrderDTO orderDto)
        {
            try
            {
                if (orderDto == null)
                    throw new ArgumentNullException(nameof(orderDto));

                var order = _mapper.Map<Order>(orderDto);
                order.Status = "not paid";
                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateOrderAsync(OrderDTO orderDto)
        {
            try
            {
                if (orderDto == null)
                    throw new ArgumentNullException(nameof(orderDto));

                var existingOrder = await _unitOfWork.Orders.GetByIdAsync(orderDto.Id);
                if (existingOrder == null)
                    throw new KeyNotFoundException($"Order with ID {orderDto.Id} not found");

                var order = _mapper.Map<Order>(orderDto);
                order.Status = existingOrder.Status ?? "not paid";
                await _unitOfWork.Orders.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
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
                return false;
            }
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(id);
                if (order == null)
                    throw new KeyNotFoundException($"Order with ID {id} not found");

                return _mapper.Map<OrderDTO>(order);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAllWithUserAndItemsAsync();
                if (orders == null || !orders.Any())
                    throw new KeyNotFoundException("No orders found");

                return _mapper.Map<IEnumerable<OrderDTO>>(orders);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<OrderDTO> PlaceOrderAsync(string userId, List<CartItemDTO> cartItemDtos, string paymentToken)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");
            if (cartItemDtos == null || !cartItemDtos.Any())
                throw new ArgumentException("Cart items cannot be empty", nameof(cartItemDtos));
            
            var cartItems = _mapper.Map<List<CartItem>>(cartItemDtos);
            decimal total = cartItems.Sum(item => (decimal)item.PriceAtAdd * item.Quantity);
            if (total <= 0)
                throw new InvalidOperationException("Total amount must be greater than zero");
            
            var user = await _unitOfWork.GetContext().Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "not paid",
                StatusUpdatedAt = DateTime.UtcNow,
                TotalAmount = total,
                OrderItems = cartItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = (decimal)item.PriceAtAdd
                }).ToList()
            };

            try
            {
                using (var transaction = _unitOfWork.GetContext().Database.BeginTransaction())
                {
                    try
                    {
                        bool paymentSuccess = false;
                        if (!string.IsNullOrEmpty(paymentToken))
                        {
                            var paymentDetails = await _paymentGateway.GetPaymentDetailsAsync(paymentToken);
                            paymentSuccess = paymentDetails.Status == "succeeded";
                        }
                        
                        if (paymentSuccess)
                        {
                            order.Status = "paid";
                            
                            foreach (var item in order.OrderItems)
                            {
                                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                                if (product == null)
                                    throw new InvalidOperationException($"Product with ID {item.ProductId} not found");
                                
                                if (product.Stock < item.Quantity)
                                    throw new InvalidOperationException($"Insufficient stock for product {product.Name}");

                                product.Stock -= item.Quantity;
                                await _unitOfWork.Products.UpdateAsync(product);
                            }

                            await _unitOfWork.Orders.AddAsync(order);
                            await _unitOfWork.SaveChangesAsync();
                            
                            transaction.Commit();
                            
                            var orderDTO = _mapper.Map<OrderDTO>(order);
                            orderDTO.UserEmail = user.Email;
                            return orderDTO;
                        }
                        else
                        {
                            throw new InvalidOperationException("Payment failed");
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
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
                Console.WriteLine($"Error in GetOrdersByUserIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                    throw new KeyNotFoundException($"Order with ID {orderId} not found");

                order.Status = status;
                order.StatusUpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Orders.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAllWithUserAndItemsAsync();
                var filteredOrders = orders.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate.AddDays(1).AddTicks(-1));
                
                if (!filteredOrders.Any())
                    return new List<OrderDTO>();

                return _mapper.Map<IEnumerable<OrderDTO>>(filteredOrders);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        public async Task<IEnumerable<OrderDTO>> GetOrdersByStatusAsync(string status)
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAllWithUserAndItemsAsync();
                string statusFilter;
                
                if (status.Equals("Processing", StringComparison.OrdinalIgnoreCase) || 
                    status.Equals("Shipped", StringComparison.OrdinalIgnoreCase) || 
                    status.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
                {
                    statusFilter = "paid";
                }
                else
                {
                    statusFilter = "not paid";
                }
                
                var filteredOrders = orders.Where(o => o.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase));
                
                if (!filteredOrders.Any())
                    return new List<OrderDTO>();

                return _mapper.Map<IEnumerable<OrderDTO>>(filteredOrders);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            string statusString = status.ToString();
            return await UpdateOrderStatusAsync(orderId, statusString);
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByStatusAsync(OrderStatus status)
        {
            string statusString = status.ToString();
            return await GetOrdersByStatusAsync(statusString);
        }
    }
}