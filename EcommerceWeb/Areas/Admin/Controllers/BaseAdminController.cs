using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWeb.Areas.Admin.Controllers
{
 
    [Area("Admin")]
  
    [Authorize(Roles = "Admin")]
    public class BaseAdminController : Controller
    {
      
    }
}