using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechXpress.Models.Dto_s
{
    public class OrderItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } // Flattened from Product
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
