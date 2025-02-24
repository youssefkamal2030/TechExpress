﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechXpress.Models
{
    public class Orders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }
        [Required]
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public Users User { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        [Required]
        public string? OrderStatus { get; set; } 

        public int? TotalAmount { get; set; }

        public ICollection<OrderDetails> OrderDetails { get; set; }

    }
}
