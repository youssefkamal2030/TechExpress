using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Climate;
using System.Security.Claims;
using TechXpress.Services.Services;
using TechXpress.Models.entitis;
using TechXpress.Services.Interfaces;

namespace TechXpress.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var orders = await _orderService.GetOrdersByUserIdAsync(userId);
                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                TempData["ErrorMessage"] = "An error occurred while retrieving your orders.";
                return View(new List<TechXpress.Models.Dto_s.OrderDTO>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var order = await _orderService.GetOrderByIdAsync(id);
                
                // Verify the order belongs to the current user
                if (order.UserId != userId)
                {
                    _logger.LogWarning($"User {userId} attempted to access order {id} which belongs to user {order.UserId}");
                    return RedirectToAction("Index");
                }
                
                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order details");
                TempData["ErrorMessage"] = "An error occurred while retrieving order details.";
                return RedirectToAction("Index");
            }
        }
    }
}