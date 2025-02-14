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

    }
}
