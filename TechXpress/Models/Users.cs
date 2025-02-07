using Microsoft.AspNetCore.Identity;
namespace TechXpress.Models
{
    public class Users: IdentityUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
