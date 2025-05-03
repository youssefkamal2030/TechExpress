using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TechXpress.Models.entitis;

namespace TechXpress.Models.Dto_s
{
    public class OrderDTO
    {
        public int Id { get; set; }
        
        public string UserId { get; set; }
        
        [Display(Name = "User Email")]
        public string UserEmail { get; set; }
        
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
        
        [Display(Name = "Status")]
        public OrderStatus Status { get; set; }
        
        [Display(Name = "Status Updated At")]
        public DateTime? StatusUpdatedAt { get; set; }
        
        [Display(Name = "Total Amount")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalAmount { get; set; }
        
        public string ProductName { get; set; }

        public UserDTO User { get; set; }
        
        public ICollection<OrderItemDTO> OrderItems { get; set; }
    }
}
