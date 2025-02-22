using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechXpress.Models;

namespace TechXpress.DataAccess.Dbinitializer
{
    public class Dbinitializer : IDbinitializer
    {
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Context _db;

        public Dbinitializer(UserManager<Users> userManager,
            RoleManager<IdentityRole> roleManager,
            Context db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }
            if (!_roleManager.RoleExistsAsync("Customer").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Customer")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Seller")).GetAwaiter().GetResult();

                _userManager.CreateAsync(new Users
                {
                    UserName = "YoussefKamal",
                    Password = "123456789k",
                    Email = "Youssefkamal@gmail.com",
                    PhoneNumber = "1234567890",
                }
                , "123456789k").GetAwaiter().GetResult();

                Users user = _db.Users.FirstOrDefault(p => p.Email == "Youssefkamal@gmail.com");
                _userManager.AddToRoleAsync(user, "Admin");

            }
            return;
        }
    }
}
