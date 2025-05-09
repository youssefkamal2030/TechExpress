using Microsoft.AspNetCore.Identity;

namespace TechXpress.Models.entitis
{
    public class User : IdentityUser
    {
       public string password { get; set; }
        public string ProfileImageUrl { get; set; }

    }
}