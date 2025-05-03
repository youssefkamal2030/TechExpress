using TechXpress.Models.entitis;

namespace TechXpress.Models.entitis
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsAvailable { get; set; } = true;
        public virtual Category Category { get; set; }
    }
}