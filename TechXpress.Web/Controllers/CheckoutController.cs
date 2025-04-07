using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using TechXpress.Models.entitis;
using TechXpress.Services.Interfaces;
using TechXpress.Models.Dto_s;

namespace TechXpress.Web.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IShoppingCartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(
            IShoppingCartService cartService,
            IOrderService orderService,
            IConfiguration configuration,
            ILogger<CheckoutController> logger)
        {
            _cartService = cartService;
            _orderService = orderService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                
                if (cart == null || !cart.Items.Any())
                {
                    TempData["ErrorMessage"] = "Your cart is empty. Please add items before checkout.";
                    return RedirectToAction("Index", "Cart");
                }

                ViewBag.StripePublishableKey = _configuration["Stripe:PublishableKey"];
                return View(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading checkout page");
                TempData["ErrorMessage"] = "An error occurred while loading the checkout page.";
                return RedirectToAction("Index", "Cart");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var cart = await _cartService.GetCartByUserIdAsync(userId);

                if (cart == null || !cart.Items.Any())
                {
                    return BadRequest(new { error = "Cart is empty" });
                }

                var lineItems = new List<SessionLineItemOptions>();

                foreach (var item in cart.Items)
                {
                    // Log item details for debugging
                    _logger.LogInformation($"Processing cart item: ID={item.Id}, ProductId={item.ProductId}, Name={item.ProductName}, Price={item.PriceAtAdd}");

                    if (string.IsNullOrEmpty(item.ProductName))
                    {
                        _logger.LogWarning($"Cart item {item.Id} has no product name");
                        continue; // Skip items with no name instead of failing
                    }

                    if (item.PriceAtAdd <= 0)
                    {
                        _logger.LogWarning($"Cart item {item.Id} has invalid price: {item.PriceAtAdd}");
                        continue; // Skip items with invalid price
                    }

                    var lineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.PriceAtAdd * 100), // Convert to cents
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName ?? $"Product {item.ProductId}" // Fallback name if ProductName is null
                            }
                        },
                        Quantity = item.Quantity
                    };

                    lineItems.Add(lineItem);
                }

                if (!lineItems.Any())
                {
                    return BadRequest(new { error = "No valid items in cart" });
                }

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,
                    Mode = "payment",
                    SuccessUrl = Url.Action("Success", "Checkout", new { sessionId = "{CHECKOUT_SESSION_ID}" }, Request.Scheme),
                    CancelUrl = Url.Action("Index", "Cart", null, Request.Scheme),
                    CustomerEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
                    Metadata = new Dictionary<string, string>
                    {
                        { "userId", userId }
                    }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                return Json(new { id = session.Id });
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error creating checkout session: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating checkout session");
                return BadRequest(new { error = "An error occurred while creating the checkout session." });
            }
        }

        public async Task<IActionResult> Success(string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    return RedirectToAction("Index", "Cart");
                }

                var service = new SessionService();
                var session = await service.GetAsync(sessionId);

                // Verify the session belongs to the current user
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (session.Metadata["userId"] != userId)
                {
                    _logger.LogWarning($"Session {sessionId} does not belong to user {userId}");
                    return RedirectToAction("Index", "Cart");
                }

                if (session.PaymentStatus == "paid")
                {
                    var cart = await _cartService.GetCartByUserIdAsync(userId);
                    
                    if (cart != null && cart.Items.Any())
                    {
                        // Convert CartItemDTO to CartItem
                        var cartItems = cart.Items.Select(dto => new CartItem
                        {
                            ProductId = dto.ProductId,
                            Quantity = dto.Quantity,
                            PriceAtAdd = dto.PriceAtAdd
                        }).ToList();

                        var order = await _orderService.PlaceOrderAsync(userId, cartItems, session.PaymentIntentId);
                        if (order.Status == "Paid")
                        {
                            return View("Success", order);
                        }
                    }
                }

                TempData["ErrorMessage"] = "Payment was not successful. Please try again.";
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing successful payment");
                TempData["ErrorMessage"] = "An error occurred while processing your payment.";
                return RedirectToAction("Index", "Cart");
            }
        }
    }
} 