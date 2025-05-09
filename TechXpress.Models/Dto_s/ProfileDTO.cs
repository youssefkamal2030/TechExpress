using System.ComponentModel.DataAnnotations;

namespace TechXpress.Models.Dto_s
{
    public class ProfileDTO
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
    }
} 