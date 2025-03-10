using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Models.entitis;

namespace TechXpress.DataAccess.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterUserAsync(User user);
        Task<string> LoginUserAsync(User user);


    }
}
