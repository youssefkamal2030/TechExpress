using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TechXpress.Models
{
    public class Context : IdentityDbContext<Users>
    {
       public Context(DbContextOptions<Context> options) : base(options) { }
       public DbSet<Users> Users { get; set; }
       public DbSet<Orders> Orders { get; set; }
       public DbSet<OrderDetails> OrdersDetails { get; set; }
       public DbSet<Products> Products { get; set; }
       public DbSet<Categories> Categories { get; set; }

       
        protected override void OnModelCreating(ModelBuilder builder)
        {
            Console.WriteLine("Seeding Database...");
            base.OnModelCreating(builder);
            builder.Entity<Categories>().HasData(
                    new Categories { CategoryID = 1 , CategoryName = "Mobils", Description="stolen moblies"},
                     new Categories { CategoryID = 2, CategoryName = "Computers", Description = "stolen Computers" },
                     new Categories { CategoryID = 3, CategoryName = "TVs", Description = "stolen TVs" },
                    new Categories { CategoryID = 4, CategoryName = "Laptops", Description = "stolen Laptops" });
            builder.Entity<Products>().HasData(
    new Products
    {
        ProductID = 1,
        CategoryID = 1, // Mobils
        ProductName = "Samsung Galaxy S23",
        Price = 900,
        Stock = 10,
        ImageURL = "images/products/samsung-s23.jpg",
        CreatedAt = DateTime.UtcNow
    },
    new Products
    {
        ProductID = 2,
        CategoryID = 1, // Mobils
        ProductName = "iPhone 14 Pro",
        Price = 1200,
        Stock = 5,
        ImageURL = "images/products/iphone-14-pro.jpg",
        CreatedAt = DateTime.UtcNow
    },
    new Products
    {
        ProductID = 3,
        CategoryID = 2, 
        ProductName = "MacBook Pro",
        Price = 2000,
        Stock = 2,
        ImageURL = "images/products/macbook-pro.jpg",
        CreatedAt = DateTime.UtcNow
    },
    new Products
    {
        ProductID = 4,
        CategoryID = 3,
        ProductName = "Samsung 50\" LED TV",
        Price = 600,
        Stock = 8,
        ImageURL = "images/products/samsung-50-led.jpg",
        CreatedAt = DateTime.UtcNow
    },
    new Products
    {
        ProductID = 5,
        CategoryID = 4,
        ProductName = "HP Pavilion 15",
        Price = 750,
        Stock = 15,
        ImageURL = "images/products/hp-pavilion-15.jpg",
        CreatedAt = DateTime.UtcNow
    }
);

        }

    }
}
