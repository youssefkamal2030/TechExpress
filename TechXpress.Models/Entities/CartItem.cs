using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechXpress.Models.entitis
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int ShoppingCartId { get; set; }
        [ForeignKey(nameof(ShoppingCartId))]
        public ShoppingCart Cart { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }

        public decimal? PriceAtAdd { get; set; }
    }
}