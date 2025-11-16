using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreshDrink.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // chỉ Admin xem được
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Admin Dashboard";
            return View(); // Areas/Admin/Views/Home/Index.cshtml
        }
    }
}
