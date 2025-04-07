using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechXpress.Models.entitis
{
    public class Payment
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "usd";

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public string OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }

        [StringLength(450)]
        public string CustomerId { get; set; }

        [StringLength(500)]
        public string ErrorMessage { get; set; }

        // Stripe specific fields
        [StringLength(100)]
        public string StripePaymentIntentId { get; set; }

        [StringLength(100)]
        public string StripeCustomerId { get; set; }

        [StringLength(100)]
        public string StripeSessionId { get; set; }
    }
}