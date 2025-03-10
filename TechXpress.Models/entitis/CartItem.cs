using System.ComponentModel.DataAnnotations;

namespace TechXpress.Models.entitis
{
    public class CartItem
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }
        [Required]
        [MaxLength(50)]
        public decimal Price { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}