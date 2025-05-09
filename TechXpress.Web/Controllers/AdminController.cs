using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechXpress.Models.Dto_s;
using TechXpress.Models.entitis;
using TechXpress.Services.Interfaces;

namespace TechXpress.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IAuthService _authService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IProductService productService,
            IOrderService orderService,
            IAuthService authService,
            ILogger<AdminController> logger)
        {
            _productService = productService;
            _orderService = orderService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
           
            try
            {
                var products = await _productService.GetAllProductsAsync();
                var categories = await _productService.GetAllCategoriesAsync();
                var orders = await _orderService.GetAllOrdersAsync();
                var users = await _authService.GetAllUsersAsync();

                ViewBag.TotalProducts = products?.Count() ?? 0;
                ViewBag.TotalCategories = categories?.Count() ?? 0;
                ViewBag.TotalOrders = orders?.Count() ?? 0;
                ViewBag.TotalUsers = users?.Count() ?? 0;
                
                var recentOrders = orders?
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToList() ?? new List<OrderDTO>();
                
                var salesData = new Dictionary<string, decimal>();
                var currentDate = DateTime.UtcNow;
                for (int i = 5; i >= 0; i--)
                {
                    var month = currentDate.AddMonths(-i);
                    var monthName = month.ToString("MMM");
                    
                    decimal monthlySales = orders?
                        .Where(o => o.OrderDate.Year == month.Year && o.OrderDate.Month == month.Month)
                        .Sum(o => o.TotalAmount) ?? 0;
                    
                    salesData.Add(monthName, monthlySales);
                }
                
                ViewBag.ChartLabels = salesData.Keys.ToArray();
                ViewBag.ChartData = salesData.Values.ToArray();
                
                return View(recentOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                TempData["Error"] = $"Error loading dashboard: {ex.Message}";
                
                ViewBag.TotalProducts = 0;
                ViewBag.TotalCategories = 0;
                ViewBag.TotalOrders = 0;
                ViewBag.TotalUsers = 0;
                ViewBag.ChartLabels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" };
                ViewBag.ChartData = new decimal[] { 0, 0, 0, 0, 0, 0 };
                
                return View(new List<OrderDTO>());
            }
        }

        public async Task<IActionResult> Products()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return View(products ?? new List<ProductDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                TempData["Error"] = $"Error loading products: {ex.Message}";
                return View(new List<ProductDTO>());
            }
        }

        public async Task<IActionResult> CreateProduct()
        {
            try
            {
                var categories = await _productService.GetAllCategoriesAsync();
                ViewBag.Categories = categories ?? new List<CategoryDTO>();
                return View(new ProductDTO { IsAvailable = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories for product creation");
                TempData["Error"] = $"Error loading categories: {ex.Message}";
                ViewBag.Categories = new List<CategoryDTO>();
                return View(new ProductDTO { IsAvailable = true });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductDTO product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.AddProductAsync(product);
                    TempData["Success"] = "Product created successfully!";
                    return RedirectToAction(nameof(Products));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating product");
                    ModelState.AddModelError("", ex.Message);
                }
            }

            try
            {
                ViewBag.Categories = await _productService.GetAllCategoriesAsync() ?? new List<CategoryDTO>();
            }
            catch
            {
                ViewBag.Categories = new List<CategoryDTO>();
            }
            
            return View(product);
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    TempData["Error"] = $"Product with ID {id} not found";
                    return RedirectToAction(nameof(Products));
                }
                
                var categories = await _productService.GetAllCategoriesAsync();
                ViewBag.Categories = categories ?? new List<CategoryDTO>();
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product for editing");
                TempData["Error"] = $"Error loading product: {ex.Message}";
                return RedirectToAction(nameof(Products));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductDTO product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.UpdateProductAsync(product);
                    TempData["Success"] = "Product updated successfully!";
                    return RedirectToAction(nameof(Products));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating product");
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                TempData["Success"] = "Product deleted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product");
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Products));
        }

        public async Task<IActionResult> Categories()
        {
            try
            {
                var categories = await _productService.GetAllCategoriesAsync();
                return View(categories ?? new List<CategoryDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories");
                TempData["Error"] = $"Error loading categories: {ex.Message}";
                return View(new List<CategoryDTO>());
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CategoryDTO category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.AddCategoryAsync(category);
                    TempData["Success"] = "Category created successfully!";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating category");
                    TempData["Error"] = ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Please check the form and try again";
            }
            
            return RedirectToAction(nameof(Categories));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(CategoryDTO category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.UpdateCategoryAsync(category);
                    TempData["Success"] = "Category updated successfully!";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating category");
                    TempData["Error"] = ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Please check the form and try again";
            }
            
            return RedirectToAction(nameof(Categories));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _productService.DeleteCategoryAsync(id);
                TempData["Success"] = "Category deleted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Categories));
        }

        public async Task<IActionResult> Orders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return View(orders ?? new List<OrderDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading orders");
                TempData["Error"] = $"Error loading orders: {ex.Message}";
                return View(new List<OrderDTO>());
            }
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    TempData["Error"] = $"Order with ID {id} not found";
                    return RedirectToAction(nameof(Orders));
                }
                
                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order details");
                TempData["Error"] = $"Error loading order details: {ex.Message}";
                return RedirectToAction(nameof(Orders));
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            try
            {
                if (string.IsNullOrEmpty(status))
                {
                    TempData["Error"] = "Order status cannot be empty";
                    return RedirectToAction(nameof(Orders));
                }
                
                // Parse string to enum
                if (!Enum.TryParse<OrderStatus>(status, out var orderStatus))
                {
                    TempData["Error"] = $"Invalid order status: {status}";
                    return RedirectToAction(nameof(Orders));
                }
                
                var success = await _orderService.UpdateOrderStatusAsync(orderId, orderStatus);
                
                if (success)
                {
                    TempData["Success"] = $"Order #{orderId} status updated to {status}";
                }
                else
                {
                    TempData["Error"] = $"Failed to update order #{orderId} status";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status");
                TempData["Error"] = $"Error updating order status: {ex.Message}";
            }
            
            return RedirectToAction(nameof(Orders));
        }
        
        public async Task<IActionResult> Users()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();
                return View(users ?? new List<UserDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users");
                TempData["Error"] = $"Error loading users: {ex.Message}";
                return View(new List<UserDTO>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserRoles(string userId, List<string> roles)
        {
            try
            {
                await _authService.UpdateUserRolesAsync(userId, roles);
                TempData["Success"] = "User roles updated successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user roles");
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                await _authService.DeleteUserAsync(userId);
                TempData["Success"] = "User deleted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Users));
        }
    }
} 