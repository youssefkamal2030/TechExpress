using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechXpress.Models.entitis
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        
        public int CartId { get; set; }
        [ForeignKey(nameof(CartId))]
        public ShoppingCart Cart { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }

       
        public decimal? PriceAtAdd { get; set; }
    }
}