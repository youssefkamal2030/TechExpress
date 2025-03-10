using System.ComponentModel.DataAnnotations;

namespace TechXpress.Models.entitis
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }
     
        public virtual ICollection<Product> Products { get; set; }
    }
}