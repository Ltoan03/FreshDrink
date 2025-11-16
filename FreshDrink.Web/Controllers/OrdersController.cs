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

        // ===================== FIX ROUTE =====================
        public IActionResult MyOrders()
        {
            return RedirectToAction(nameof(Index));
        }

        // ===================== LIST ORDERS =====================
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var ordersQuery = _db.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .AsQueryable();

            if (!isAdmin)
                ordersQuery = ordersQuery.Where(o => o.UserId == userId);

            var orders = await ordersQuery
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderSummaryVm
                {
                    Id = o.Id,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status.ToString(),
                    Total = o.Total,
                    ItemsCount = o.Items.Count,
                   UserName = o.User != null ? o.User.UserName : "Không xác định"
                })
                .ToListAsync();

            return View("MyOrders", orders);
        }

        // ===================== DETAILS =====================
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = OrderStatus.Approved;
            await _db.SaveChangesAsync();

            TempData["ok"] = $"✔ Đơn hàng #{id} đã duyệt.";
            return RedirectToAction(nameof(Index));
        }

        // ===================== REJECT =====================
        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = OrderStatus.Rejected;
            await _db.SaveChangesAsync();

            TempData["ok"] = $"⚠ Đơn hàng #{id} đã bị từ chối.";
            return RedirectToAction(nameof(Index));
        }

        // ===================== DELETE =====================
        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            _db.OrderItems.RemoveRange(order.Items);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();

            TempData["ok"] = $"🗑 Đã xóa đơn hàng #{id}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
