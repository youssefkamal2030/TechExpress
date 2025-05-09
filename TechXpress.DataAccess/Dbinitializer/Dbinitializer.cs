using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechXpress.DataAccess.Data;
using TechXpress.Models.entitis;

namespace TechXpress.DataAccess.Dbinitializer
{
    public class Dbinitializer : IDbinitializer
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TechXpressDbContext _db;
        private readonly ILogger<Dbinitializer> _logger;
        private static bool _isInitialized = false;

        public Dbinitializer(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            TechXpressDbContext db,
            ILogger<Dbinitializer> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            // Skip initialization if already done
            if (_isInitialized)
            {
                return;
            }

            try
            {
                // Check if database exists and has been migrated
                bool databaseExists = await _db.Database.CanConnectAsync();
                if (!databaseExists)
                {
                    _logger.LogInformation("Database does not exist. Creating and applying migrations...");
                }
                
                // Only apply migrations if there are pending ones
                if (databaseExists && !(await _db.Database.GetPendingMigrationsAsync()).Any())
                {
                    _logger.LogInformation("No pending migrations. Skipping database initialization.");
                    
                    // Check if roles exist to determine if initial data is seeded
                    if (await _roleManager.RoleExistsAsync("Customer"))
                    {
                        _isInitialized = true;
                        return;
                    }
                }
                else
                {
                    await _db.Database.MigrateAsync();
                    _logger.LogInformation("Database migrations applied successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during database initialization.");
                return;
            }

            // Seed initial data
            await SeedRolesAndAdminUserAsync();
            await SeedCategoriesAsync();
            await SeedProductsAsync();
            
            _isInitialized = true;
            _logger.LogInformation("Database initialization completed successfully.");
        }
        
        private async Task SeedRolesAndAdminUserAsync()
        {
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
                
                _logger.LogInformation("Roles and admin user created successfully.");
            }
        }
        
        private async Task SeedCategoriesAsync()
        {
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
                
                _logger.LogInformation("Categories seeded successfully.");
            }
        }
        
        private async Task SeedProductsAsync()
        {
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
                
                _logger.LogInformation("Products seeded successfully.");
            }
        }
    }
}