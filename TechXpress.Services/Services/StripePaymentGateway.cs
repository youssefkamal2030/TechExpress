using Stripe;
using TechXpress.DataAccess.Interfaces;

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
                Amount = (long)(amount * 100), // Convert to cents
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
    }
}