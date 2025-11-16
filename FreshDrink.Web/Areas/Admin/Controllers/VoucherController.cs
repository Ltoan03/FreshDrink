using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FreshDrink.Data;
using FreshDrink.Data.Models;

namespace FreshDrink.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VoucherController : Controller
    {
        private readonly FreshDrinkDbContext _db;

        public VoucherController(FreshDrinkDbContext db)
        {
            _db = db;
        }

        // -------- LIST --------
        public async Task<IActionResult> Index()
        {
            var vouchers = await _db.Vouchers
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();

            return View(vouchers);
        }

        // -------- CREATE --------
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Voucher voucher)
        {
            if (!ModelState.IsValid)
                return View(voucher);

            voucher.CreatedAt = DateTime.Now;
            _db.Vouchers.Add(voucher);
            await _db.SaveChangesAsync();

            TempData["success"] = "🎉 Voucher đã được tạo!";
            return RedirectToAction(nameof(Index));
        }

        // -------- EDIT --------
        public async Task<IActionResult> Edit(int id)
        {
            var voucher = await _db.Vouchers.FindAsync(id);
            if (voucher == null) return NotFound();

            return View(voucher);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Voucher voucher)
        {
            if (!ModelState.IsValid)
                return View(voucher);

            _db.Vouchers.Update(voucher);
            await _db.SaveChangesAsync();

            TempData["success"] = "✏ Voucher đã được cập nhật!";
            return RedirectToAction(nameof(Index));
        }

        // -------- DELETE --------
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var voucher = await _db.Vouchers.FindAsync(id);
            if (voucher == null) return NotFound();

            _db.Vouchers.Remove(voucher);
            await _db.SaveChangesAsync();

            TempData["success"] = "🗑 Voucher đã được xóa!";
            return RedirectToAction(nameof(Index));
        }
    }
}
