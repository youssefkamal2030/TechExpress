using TechXpress.Models;

namespace TechXpress.Repositories
{
    public class ProductsRepo : Repository<Products>
    {
        public ProductsRepo(Context context) : base(context)
        {
        }
        public List<Products> Get_Products_by_Category(int categoryID)
        {

            return _context.Products.Where(p => p.CategoryID == categoryID).ToList();

        }
    }
}
