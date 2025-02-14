using Microsoft.AspNetCore.Identity;
using TechXpress.Models;
namespace TechXpress.Repositories
{
    public class UsersRepo
    {
        private readonly UserManager<Users> _userManager;
        public UsersRepo(UserManager<Users> userManager)
        {
            _userManager = userManager;
        }
        public void Create(Users user, string password)
        {
           _userManager.CreateAsync(user, password).Wait();
        }
        public void Update(Users user)
        {
            _userManager.UpdateAsync(user).Wait();
        }
        public void Delete(Users user)
        {
            _userManager.DeleteAsync(user).Wait();
        }
        public Users GetByID(string id)
        {
            return _userManager.FindByIdAsync(id).Result;
        }
        public List<Users> GetAll()
        {
            return _userManager.Users.ToList();
        }
        public Users GetById(string id)
        {
            return _userManager.FindByIdAsync(id).Result;

        }
        public Task<Users> GetByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

    }
}
