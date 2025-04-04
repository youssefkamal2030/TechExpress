using Stripe;
using TechXpress.Services.Interfaces;

namespace TechXpress.Services
{
    public class StripePaymentGateway : IPaymentGateway
    {
        private readonly string _apiKey;

        public StripePaymentGateway(string apiKey)
        {
            _apiKey = apiKey;
            StripeConfiguration.ApiKey = _apiKey;
        }

        public async Task<bool> ProcessPaymentAsync(decimal amount, string token)
        {
            var options = new ChargeCreateOptions
            {
                Amount = (long)(amount * 100), 
                Currency = "usd",
                Source = token,
                Description = "TechXpress Order"
            };
            var service = new ChargeService();
            try
            {
                var charge = await service.CreateAsync(options);
                return charge.Status == "succeeded";
            }
            catch (StripeException)
            {
                return false;
            }
        }

        public async Task<bool> RefundAsync(string transactionId, decimal amount)
        {
            var options = new RefundCreateOptions
            {
                Charge = transactionId,
                Amount = (long)(amount * 100) 
            };
            var service = new RefundService();
            try
            {
                var refund = await service.CreateAsync(options);
                return refund.Status == "succeeded";
            }
            catch (StripeException)
            {
                return false;
            }
        }
    }
}