using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TechXpress.Services.Interfaces; // Assuming this is your service namespace

namespace TechXpress.Web.ViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly IShoppingCartService _cartService;

        public CartSummaryViewComponent(IShoppingCartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = UserClaimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);
            int itemCount = 0;
            if (!string.IsNullOrEmpty(userId))
            {
                itemCount = await _cartService.GetCartCountAsync(userId);
            }
            return Content(itemCount.ToString());
        }
    }
}