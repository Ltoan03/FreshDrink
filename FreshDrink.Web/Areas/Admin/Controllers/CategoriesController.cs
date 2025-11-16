using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FreshDrink.Data;
using FreshDrink.Data.Models;

namespace FreshDrink.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly FreshDrinkDbContext _db;

        public CategoriesController(FreshDrinkDbContext db)
        {
            _db = db;
        }

        // ========== LIST ==========
        public async Task<IActionResult> Index()
        {
            var list = await _db.Categories
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return View(list);
        }

        // ========== CREATE ==========
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _db.Categories.Add(model);
            await _db.SaveChangesAsync();

            TempData["success"] = "Tạo danh mục thành công!";
            return RedirectToAction("Index");
        }

        // ========== EDIT ==========
        public async Task<IActionResult> Edit(int id)
        {
            var cate = await _db.Categories.FindAsync(id);
            if (cate == null) return NotFound();

            return View(cate);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _db.Categories.Update(model);
            await _db.SaveChangesAsync();

            TempData["success"] = "Cập nhật danh mục thành công!";
            return RedirectToAction("Index");
        }

        // ========== DELETE ==========
        public async Task<IActionResult> Delete(int id)
        {
            var cate = await _db.Categories.FindAsync(id);
            if (cate == null) return NotFound();

            return View(cate);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var cate = await _db.Categories.FindAsync(id);
            if (cate == null) return NotFound();

            _db.Categories.Remove(cate);
            await _db.SaveChangesAsync();

            TempData["success"] = "Xóa danh mục thành công!";
            return RedirectToAction("Index");
        }
    }
}
