using TechXpress.Models;
using TechXpress.Repositories;
namespace TechXpress.Repositories
{
    public class OrdersRepo : Repository<Orders>
    {
        public OrdersRepo(Context context) : base(context)
        {
        }
    }
}