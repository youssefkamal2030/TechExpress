using System;

namespace TechXpress.Models.Dto_s
{
    public class PaymentDTO
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public string ErrorMessage { get; set; }
    }
} 