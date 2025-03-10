namespace TechXpress.DataAccess.Interfaces
{ 
    public interface IPaymentGateway
    {
        Task<bool> ProcessPaymentAsync(decimal amount, string token);
        Task<bool> RefundAsync(string transactionId, decimal amount);
    }
}