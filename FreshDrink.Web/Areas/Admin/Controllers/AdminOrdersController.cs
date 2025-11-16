using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FreshDrink.Data;
using FreshDrink.Data.Models;
using FreshDrink.Data.Identity;

namespace FreshDrink.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly FreshDrinkDbContext _db;

        public AdminOrdersController(FreshDrinkDbContext db)
        {
            _db = db;
        }

        // LIST ORDERS
        public async Task<IActionResult> Index()
        {
            var orders = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Drink)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        // DETAILS
        public async Task<IActionResult> Details(int id)
        {
            var order = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Drink)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }

        // APPROVE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            if (order.Status != OrderStatus.Pending)
            {
                TempData["error"] = "⚠ Đơn này đã xử lý rồi!";
                return RedirectToAction(nameof(Index));
            }

            order.Status = OrderStatus.Approved;
            await _db.SaveChangesAsync();

            TempData["success"] = $"✔ Đã duyệt đơn #{order.Id}.";
            return RedirectToAction(nameof(Index));
        }

        // REJECT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            if (order.Status != OrderStatus.Pending)
            {
                TempData["error"] = "⚠ Không thể từ chối vì đơn đã xử lý!";
                return RedirectToAction(nameof(Index));
            }

            order.Status = OrderStatus.Rejected;
            await _db.SaveChangesAsync();

            TempData["warning"] = $"❌ Đơn #{order.Id} đã bị từ chối!";
            return RedirectToAction(nameof(Index));
        }

        // DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            _db.OrderItems.RemoveRange(order.Items);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();

            TempData["success"] = $"🗑 Đã xóa đơn #{order.Id}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
