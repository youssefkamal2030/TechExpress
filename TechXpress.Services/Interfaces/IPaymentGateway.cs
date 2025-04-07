using TechXpress.Models.Dto_s;

namespace TechXpress.Services.Interfaces
{
    public interface IPaymentGateway
    {
        Task<bool> ProcessPaymentAsync(decimal amount, string token);
        Task<bool> RefundAsync(string transactionId, decimal amount);
        Task<PaymentDTO> CreatePaymentIntentAsync(decimal amount, string customerId = null);
        Task<PaymentDTO> GetPaymentDetailsAsync(string paymentIntentId);
        Task<PaymentDTO> CreateCheckoutSessionAsync(decimal amount, string successUrl, string cancelUrl, string customerEmail = null);
    }
}