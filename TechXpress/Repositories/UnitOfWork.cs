using Microsoft.AspNetCore.Identity;
using TechXpress.Models;

namespace TechXpress.Repositories
{
    public class UnitOfWork
    {
        private readonly Context _context;

        public CategoriesRepo Categories { get; }
        public ProductsRepo Products { get; }
        public OrdersRepo Orders { get; }
        public OrdersDetailsRepo OrdersDetails { get; }
        public UsersRepo Users { get; }

        public UnitOfWork(Context context, UserManager<Users> userManager)
        {
            _context = context;
            Categories = new CategoriesRepo(context);
            Products = new ProductsRepo(context);
            Orders = new OrdersRepo(context);
            OrdersDetails = new OrdersDetailsRepo(context);
            Users = new UsersRepo(userManager);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }

}
