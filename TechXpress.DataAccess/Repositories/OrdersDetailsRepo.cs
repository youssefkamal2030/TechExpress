using TechXpress.Models;
namespace TechXpress.Repositories
{
    public class OrdersDetailsRepo : Repository<OrderDetails>
    {
        public OrdersDetailsRepo(Context context) : base(context)
        {
        }
    }
}
