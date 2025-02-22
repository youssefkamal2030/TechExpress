using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TechXpress.Filters
{
    public class HandleError:Attribute,IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            ContentResult contentResult = new ContentResult();
            contentResult.Content = "Something went wrong ";
           context.Result = contentResult;
        }
    }
}
