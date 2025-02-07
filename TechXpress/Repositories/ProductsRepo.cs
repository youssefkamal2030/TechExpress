using TechXpress.Models;

namespace TechXpress.Repositories
{
    public class ProductsRepo : Repository<Products>
    {
        public ProductsRepo(Context context) : base(context)
        {
        }
    }   
}
