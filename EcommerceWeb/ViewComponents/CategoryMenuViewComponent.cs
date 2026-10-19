using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWeb.ViewComponents
{
    public class CategoryMenuViewComponent(ICategoryService categoryService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await categoryService.GetAllCategoriesAsync();
            return View(categories);
        }
    }
}