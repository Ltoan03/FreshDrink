using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FreshDrink.Data;
using FreshDrink.Data.Models;
using FreshDrink.Web.ViewModels;
using FreshDrink.Data.Identity;

namespace FreshDrink.Web.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly FreshDrinkDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public OrdersController(FreshDrinkDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ===================== REDIRECT TO LIST =====================
        public IActionResult MyOrders() => RedirectToAction(nameof(Index));

        // ===================== LIST ORDERS =====================
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            var orders = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .Where(o => isAdmin || o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Include(o => o.User)
                .Select(o => new OrderSummaryVm
                {
                    Id = o.Id,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status.ToString(),
                    Total = o.Total,
                    ItemsCount = o.Items.Count,
                    UserName = o.User!.UserName 
                })
                .ToListAsync();

            return View("MyOrders", orders);
        }

        // ===================== DETAILS =====================
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            var order = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null || (!isAdmin && order.UserId != userId))
                return NotFound();

            var vm = new OrderDetailVm
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                Total = order.Total,
                UserName = order.User?.UserName ?? "Không xác định",
                Items = order.Items.Select(i => new OrderDetailItemVm
                {
                    Name = i.Name,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    LineTotal = i.LineTotal
                }).ToList()
            };

            return View(vm);
        }

        // ===================== APPROVE =====================
        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var updated = await UpdateOrderStatus(id, OrderStatus.Approved, $"✔ Đơn hàng #{id} đã duyệt.");
            return updated;
        }

        // ===================== REJECT =====================
        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var updated = await UpdateOrderStatus(id, OrderStatus.Rejected, $"⚠ Đơn hàng #{id} đã bị từ chối.");
            return updated;
        }

        private async Task<IActionResult> UpdateOrderStatus(int id, OrderStatus status, string message)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.Status = status;
            await _db.SaveChangesAsync();

            TempData["ok"] = message;
            return RedirectToAction(nameof(Index));
        }

        // ===================== DELETE =====================
        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            _db.OrderItems.RemoveRange(order.Items);
            _db.Orders.Remove(order);

            await _db.SaveChangesAsync();

            TempData["ok"] = $"🗑 Đã xóa đơn hàng #{id}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
