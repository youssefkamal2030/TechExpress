using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using TechXpress.Models.entitis;
using TechXpress.Services.Interfaces;
using TechXpress.Models.Dto_s;
using System.Security.Claims;

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
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value; //gets the userID that stored during the log in the claims table
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

                var lineItems = new List<SessionLineItemOptions>(); // a list of items with specific details requierd for stripe api 

                // this iterates over all the items in the cart and convert them to a sessionLineItem that stripe api can understand
                foreach (var item in cart.Items)
                {
                   
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

                var options = new SessionCreateOptions //options for handling the payment and how to handle success and faluire
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,
                    Mode = "payment",
                    SuccessUrl = $"{Request.Scheme}://{Request.Host}/Checkout/Success?sessionId={{CHECKOUT_SESSION_ID}}",
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
                    TempData["ErrorMessage"] = "Invalid checkout session.";
                    return RedirectToAction("Index", "Cart");
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "User not authenticated.";
                    return RedirectToAction("Login", "Account");
                }

                var service = new SessionService();
                var session = await service.GetAsync(sessionId);
                
                var sessionUserId = session.Metadata.ContainsKey("userId") ? session.Metadata["userId"] : null;

                if (sessionUserId != userId)
                {
                    TempData["ErrorMessage"] = "This payment session doesn't belong to you.";
                    return RedirectToAction("Index", "Cart");
                }

                if (session.PaymentStatus == "paid")
                {
                    var cart = await _cartService.GetCartByUserIdAsync(userId);
                    
                    if (cart != null && cart.Items.Any())
                    {
                        var cartItems = cart.Items.Select(dto => new CartItem
                        {
                            ProductId = dto.ProductId,
                            Quantity = dto.Quantity,
                            PriceAtAdd = dto.PriceAtAdd
                        }).ToList();

                        try
                        {
                            var order = await _orderService.PlaceOrderAsync(userId, cartItems, session.PaymentIntentId ?? sessionId);
                            
                            if (order.Status == "Paid")
                            {
                                return View("Success", order);
                            }
                            
                            TempData["ErrorMessage"] = "Your payment was processed, but there was an issue with your order status.";
                            return RedirectToAction("Index", "Cart");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error creating order for user {UserId}", userId);
                            TempData["ErrorMessage"] = "There was an error processing your order. Please contact support.";
                            return RedirectToAction("Index", "Cart");
                        }
                    }
                    
                    TempData["ErrorMessage"] = "Your payment was processed, but your cart is empty.";
                    return RedirectToAction("Index", "Home");
                }
                
                TempData["ErrorMessage"] = "Your payment was not successful. Please try again.";
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