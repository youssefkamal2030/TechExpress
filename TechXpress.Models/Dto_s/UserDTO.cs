using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TechXpress.Models.Dto_s
{
    public class UserDTO
    {
        public string Id { get; set; }
        
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        
        public string Password { get; set; }
        
        public bool EmailConfirmed { get; set; }
        
        public bool PhoneNumberConfirmed { get; set; }
        
        public bool TwoFactorEnabled { get; set; }
        
        public bool LockoutEnabled { get; set; }
        
        public DateTimeOffset? LockoutEnd { get; set; }
        
        // New properties for enhanced user management
        [Display(Name = "User Roles")]
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        
        [Display(Name = "Account Locked")]
        public bool IsLocked { get; set; }
    }
} 