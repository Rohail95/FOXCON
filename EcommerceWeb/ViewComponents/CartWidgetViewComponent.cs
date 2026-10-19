using BusinessLayer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using DatabaseLayer.Models;
using Microsoft.AspNetCore.Http;

namespace EcommerceWeb.ViewComponents
{
    public class CartWidgetViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartWidgetViewComponent(ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string cartIdentifier = GetCartIdentifier();
            int cartItemCount = 0;

            if (!string.IsNullOrEmpty(cartIdentifier))
            {
                var cart = await _cartService.GetCartByIdentifierAsync(cartIdentifier);
                if (cart != null)
                {
                    cartItemCount = cart.CartItems.Sum(item => item.Quantity);
                }
            }

            return View(cartItemCount);
        }

        private string GetCartIdentifier()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return _userManager.GetUserId(UserClaimsPrincipal);
            }

            return HttpContext.Session.GetString("CartSessionId");
        }
    }
}