using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace TechXpress.Models.Dto_s
{
    public class ShoppingCartDTO
    {
    
            public int Id { get; set; }
            public string ApplicationUserId { get; set; }
            public List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();

            public decimal TotalPrice { get; set; } 
        
    }

}
