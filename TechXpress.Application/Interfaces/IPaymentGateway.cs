namespace TechXpress.Application.Interfaces
{
    public interface IPaymentGateway
    {
        Task<bool> ProcessPaymentAsync(decimal amount, string token);
    }
}