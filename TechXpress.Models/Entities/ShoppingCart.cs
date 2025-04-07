using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Models.entitis;

namespace TechXpress.Models.entitis
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }

       
        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
