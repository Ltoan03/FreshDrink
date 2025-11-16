using Microsoft.AspNetCore.Mvc;
using FreshDrink.Data;
using Microsoft.EntityFrameworkCore;

namespace FreshDrink.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CartController : Controller
    {
        private readonly FreshDrinkDbContext _db;

        public CartController(FreshDrinkDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _db.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return View(orders);
        }
    }
}
