using Microsoft.AspNetCore.Identity;

namespace TechXpress.Domain.Entities
{
    public class User : IdentityUser
    {
       public string password { get; set; }
    }
}