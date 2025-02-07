using TechXpress.Models;
namespace TechXpress.Repositories
{
    public class CategoriesRepo : Repository<Categories>
    {
        public CategoriesRepo(Context context) : base(context)
        {
        }
    }
       
}
