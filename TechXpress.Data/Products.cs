using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TechXpress.Models
{
    public class Products
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }
       
        public int CategoryID { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Stock { get; set; }
        public string? ImageURL { get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey("CategoryID")]
        public Categories? Category { get; set; }

    }
}
