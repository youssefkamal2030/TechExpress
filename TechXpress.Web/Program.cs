using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechXpress.DataAccess.Interfaces;
using TechXpress.Services.Services;
using TechXpress.DataAccess.Dbinitializer;
using TechXpress.DataAccess.Data;
using TechXpress.Models.entitis;
using TechXpress.Services;
using TechXpress.DataAccess.Repositories;
using TechXpress.Services.Services;
using TechXpress.Services.Interfaces;
using TechXpress.Models.Mappings;
using Stripe;
using TechXpress.Models.Configuration;
using AutoMapper;
using Microsoft.Data.SqlClient;

namespace TechXpress
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Database Context with retry policy
            builder.Services.AddDbContext<TechXpressDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });

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
            builder.Services.AddMemoryCache();

            // Repository and Services Registration 
            builder.Services.AddScoped<IDbinitializer, Dbinitializer>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IProductService, TechXpress.Services.Services.ProductService>();
            builder.Services.AddScoped<IShoppingCartService, CartService>();
            builder.Services.AddScoped<IOrderService, TechXpress.Services.Services.OrderService>();

            // Configure Stripe Settings
            var stripeSettings = builder.Configuration.GetSection("Stripe").Get<StripeSettings>();
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = stripeSettings.SecretKey;

            // Register StripePaymentGateway with configuration
            builder.Services.AddScoped<IPaymentGateway>(provider => 
                new StripePaymentGateway(
                    stripeSettings.SecretKey,
                    provider.GetRequiredService<IMapper>()
                ));

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
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Run database seeding asynchronously
            _ = Task.Run(async () =>
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbInit = scope.ServiceProvider.GetRequiredService<IDbinitializer>();
                    await dbInit.InitializeAsync();
                }
            });

            app.Run();
        }   
    }
}
