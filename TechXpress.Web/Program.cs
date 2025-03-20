using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechXpress.DataAccess.Interfaces;
using TechXpress.Application.Services;
using TechXpress.DataAccess.Dbinitializer;
using TechXpress.DataAccess.Data;
using TechXpress.Models.entitis;
using TechXpress.Services;
using TechXpress.DataAccess.Repositories;
using TechXpress.Services.Services;
using TechXpress.Services.Interfaces;
using TechXpress.Models.Mappings;

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
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddAuthentication("CookieAuth")
            .AddCookie("CookieAuth", options =>
            {
        options.LoginPath = "/Account/Login";
            });
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = System.TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            //Repso and Services Register 
            builder.Services.AddScoped<IDbinitializer, Dbinitializer>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IShoppingCartService, CartService>();
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
            app.UseSession();
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
