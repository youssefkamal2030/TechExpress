using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Models.Dto_s;
using TechXpress.Models.entitis;

namespace TechXpress.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterUserAsync(User user);
        Task<string> LoginUserAsync(User user);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<bool> UpdateUserRolesAsync(string userId, List<string> roles);
        Task<bool> DeleteUserAsync(string userId);
    }
}
