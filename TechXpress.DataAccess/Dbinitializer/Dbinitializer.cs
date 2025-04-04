using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Initialize()
        {
            
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                
            }

            
            if (!_roleManager.RoleExistsAsync("Customer").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Customer")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();

                var adminUser = new User
                {
                    UserName = "YoussefKamal",
                    Email = "Youssefkamal@gmail.com",
                    password = "123456789k",
                    PhoneNumber = "1234567890",
                };
                _userManager.CreateAsync(adminUser, "123456789k").GetAwaiter().GetResult();

                User user = _db.Users.FirstOrDefault(p => p.Email == "Youssefkamal@gmail.com");
                _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
            }

          
            if (!_db.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Electronics" },
                    new Category { Name = "Mobils" },
                    new Category { Name = "TVs" },
                };
                _db.Categories.AddRange(categories);
                _db.SaveChanges();
            }

            // Seed products if they don't exist
            if (!_db.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        CategoryId = _db.Categories.First(c => c.Name == "Electronics").Id,
                        Name = "Smartphone",
                        Price = 599.99m,
                        Stock = 100,
                        ImageUrl = "/images/smartphone.jpg",
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        CategoryId = _db.Categories.First(c => c.Name == "Clothing").Id,
                        Name = "T-Shirt",
                        Price = 19.99m,
                        Stock = 200,
                        ImageUrl = "/images/tshirt.jpg",
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        CategoryId = _db.Categories.First(c => c.Name == "Books").Id,
                        Name = "ASP.NET Core Guide",
                        Price = 39.99m,
                        Stock = 50,
                        ImageUrl = "/images/book.jpg",
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        CategoryId = _db.Categories.First(c => c.Name == "Home & Garden").Id,
                        Name = "Plant Pot",
                        Price = 9.99m,
                        Stock = 150,
                        ImageUrl = "/images/plantpot.jpg",
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        CategoryId = _db.Categories.First(c => c.Name == "Sports").Id,
                        Name = "Football",
                        Price = 29.99m,
                        Stock = 80,
                        ImageUrl = "/images/football.jpg",
                        CreatedAt = DateTime.Now
                    }
                };
                _db.Products.AddRange(products);
                _db.SaveChanges();
            }
        }
    }
}