using Microsoft.AspNetCore.Identity;
namespace TechXpress.Models
{
    public class Users: IdentityUser
    {
      
        public string Password { get; set; }
    }
}
