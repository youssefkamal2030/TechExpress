using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechXpress.DataAccess.Data;
using TechXpress.Models.entitis;

namespace TechXpress.DataAccess.Dbinitializer
{
    public class Dbinitializer : IDbinitializer
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TechXpressDbContext _db;

        public Dbinitializer(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            TechXpressDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    await _db.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                
            }

            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("Seller"));

                var adminUser = new User
                {
                    UserName = "YoussefKamal",
                    Email = "Youssefkamal@gmail.com",
                    password = "123456789k",
                    PhoneNumber = "1234567890",
                };
                await _userManager.CreateAsync(adminUser, "123456789k");

                User user = await _db.Users.FirstOrDefaultAsync(p => p.Email == "Youssefkamal@gmail.com");
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            if (!await _db.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Electronics" },
                    new Category { Name = "Mobils" },
                    new Category { Name = "TVs" },
                };
                await _db.Categories.AddRangeAsync(categories);
                await _db.SaveChangesAsync();
            }

            if (!await _db.Products.AnyAsync())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        CategoryId = (await _db.Categories.FirstAsync(c => c.Name == "Electronics")).Id,
                        Name = "Smartphone",
                        Price = 599.99m,
                        Stock = 100,
                        ImageUrl = "/images/smartphone.jpg",
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        CategoryId = (await _db.Categories.FirstAsync(c => c.Name == "Mobils")).Id,
                        Name = "T-Shirt",
                        Price = 19.99m,
                        Stock = 200,
                        ImageUrl = "/images/tshirt.jpg",
                        CreatedAt = DateTime.Now
                    }
                };
                await _db.Products.AddRangeAsync(products);
                await _db.SaveChangesAsync();
            }
        }
    }
}