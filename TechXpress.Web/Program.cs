using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechXpress.Application.Interfaces;
using TechXpress.Application.Services;
using TechXpress.DataAccess.Dbinitializer;
using TechXpress.Domain.Entities;
using TechXpress.Infrastructure.Data;
using TechXpress.Infrastructure.Services;


namespace TechXpress
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Database Context
            builder.Services.AddDbContext<TechXpressDbContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Add Identity Services
            builder.Services.AddDefaultIdentity<User>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 0;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<TechXpressDbContext>()
            .AddDefaultTokenProviders();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/SignIn";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });
            //Repso Register in DI
            builder.Services.AddScoped<IDbinitializer, Dbinitializer>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<ICartStorage, SessionCartStorage>();
            builder.Services.AddScoped<IPaymentGateway>(sp => new StripePaymentGateway(builder.Configuration["Stripe:ApiKey"]));
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            
            SeedDb();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
            void SeedDb()
            {
                using (var Scope = app.Services.CreateScope())
                {
                    var Dbinit = Scope.ServiceProvider.GetRequiredService<IDbinitializer>();
                    Dbinit.Initialize();
                }
            }

        }
    }
}
