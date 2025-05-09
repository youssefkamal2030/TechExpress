using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using TechXpress.Models.Dto_s;

namespace TechXpress.Models.Dto_s
{
    public class ProductDTO
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        
        public string CategoryName { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        public string? Description { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }
        
        public string ImageUrl { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "Available for Purchase")]
        public bool IsAvailable { get; set; } = true;
        
        // Review-related properties
        public ICollection<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public Dictionary<int, int> RatingBreakdown { get; set; } = new Dictionary<int, int>();
    }
}
