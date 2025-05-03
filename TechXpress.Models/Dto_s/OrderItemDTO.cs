using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TechXpress.Models.Dto_s
{
    public class OrderItemDTO
    {
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        
        public int ProductId { get; set; }
        
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        
        [Display(Name = "Unit Price")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal UnitPrice { get; set; }
        
        public ProductDTO Product { get; set; }
    }
}
