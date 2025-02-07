using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TechXpress.Models
{
    public class Products
    {
        [Key]
        public int ProductID { get; set; }
       
        public int CategoryID { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }
        public string ImageURL { get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey("CategoryID")]
        public ICollection<Categories> Categories { get; set; }

    }
}
