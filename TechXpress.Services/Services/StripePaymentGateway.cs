using AutoMapper;
using Stripe;
using Stripe.Checkout;
using TechXpress.Models.Dto_s;
using TechXpress.Services.Interfaces;

namespace TechXpress.Services
{
    public class StripePaymentGateway : IPaymentGateway
    {
        private readonly string _apiKey;
        private readonly IMapper _mapper;

        public StripePaymentGateway(string apiKey, IMapper mapper)
        {
            _apiKey = apiKey;
            _mapper = mapper;
            StripeConfiguration.ApiKey = _apiKey;
        }

        public async Task<bool> ProcessPaymentAsync(decimal amount, string token)
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(amount * 100), // Convert to cents
                    Currency = "usd",
                    PaymentMethod = token,
                    Confirm = true,
                    ReturnUrl = "https://your-domain.com/checkout/success"
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                return paymentIntent.Status == "succeeded";
            }
            catch (StripeException ex)
            {
                // Log the error
                return false;
            }
        }

        public async Task<bool> RefundAsync(string transactionId, decimal amount)
        {
            try
            {
                var options = new RefundCreateOptions
                {
                    PaymentIntent = transactionId,
                    Amount = (long)(amount * 100) // Convert to cents
                };

                var service = new RefundService();
                var refund = await service.CreateAsync(options);

                return refund.Status == "succeeded";
            }
            catch (StripeException ex)
            {
                // Log the error
                return false;
            }
        }

        public async Task<PaymentDTO> CreatePaymentIntentAsync(decimal amount, string customerId = null)
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(amount * 100),
                    Currency = "usd",
                    Customer = customerId,
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                return new PaymentDTO
                {
                    Id = paymentIntent.Id,
                    Amount = amount,
                    Status = paymentIntent.Status,
                    CustomerId = customerId,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (StripeException ex)
            {
                return new PaymentDTO
                {
                    Status = "failed",
                    ErrorMessage = ex.Message,
                    CreatedAt = DateTime.UtcNow
                };
            }
        }

        public async Task<PaymentDTO> GetPaymentDetailsAsync(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentIntentId);

                return new PaymentDTO
                {
                    Id = paymentIntent.Id,
                    Amount = paymentIntent.Amount / 100m,
                    Status = paymentIntent.Status,
                    PaymentMethod = paymentIntent.PaymentMethodTypes?.FirstOrDefault(),
                    CustomerId = paymentIntent.CustomerId,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (StripeException ex)
            {
                return new PaymentDTO
                {
                    Status = "failed",
                    ErrorMessage = ex.Message,
                    CreatedAt = DateTime.UtcNow
                };
            }
        }

        public async Task<PaymentDTO> CreateCheckoutSessionAsync(decimal amount, string successUrl, string cancelUrl, string customerEmail = null)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(amount * 100),
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = "Order Total",
                                },
                            },
                            Quantity = 1,
                        },
                    },
                    Mode = "payment",
                    SuccessUrl = successUrl,
                    CancelUrl = cancelUrl,
                    CustomerEmail = customerEmail
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                return new PaymentDTO
                {
                    Id = session.Id,
                    Amount = amount,
                    Status = session.PaymentStatus,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (StripeException ex)
            {
                return new PaymentDTO
                {
                    Status = "failed",
                    ErrorMessage = ex.Message,
                    CreatedAt = DateTime.UtcNow
                };
            }
        }
    }
}