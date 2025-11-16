using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FreshDrink.Data;
using FreshDrink.Data.Models;
using FreshDrink.Web.ViewModels;

namespace FreshDrink.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DrinksController : Controller
    {
        private readonly FreshDrinkDbContext _db;
        private readonly IWebHostEnvironment _env;

        public DrinksController(FreshDrinkDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // ================= LIST =================
        public async Task<IActionResult> Index()
        {
            var drinks = await _db.Drinks
                .AsNoTracking()
                .Include(d => d.Category)
                .OrderByDescending(d => d.Id)
                .ToListAsync();

            return View(drinks);
        }

        // ================= CREATE =================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            return View(new DrinkCreateVm());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DrinkCreateVm vm)
        {
            if (vm.ImageFile == null)
                ModelState.AddModelError("ImageFile", "Vui lòng chọn ảnh!");

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(vm.CategoryId);
                return View(vm);
            }

            var drink = new Drink
            {
                Name = vm.Name.Trim(),
                Price = vm.Price,
                CategoryId = vm.CategoryId,
                Description = vm.Description,
                IsActive = vm.IsActive,
                IsFeatured = vm.IsFeatured
            };

            var url = await SaveImageAsync(vm.ImageFile!);
            drink.ImageUrl = url;

            _db.Drinks.Add(drink);
            await _db.SaveChangesAsync();

            TempData["ok"] = "Đã thêm đồ uống.";
            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT =================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var drink = await _db.Drinks.FindAsync(id);
            if (drink == null) return NotFound();

            var vm = new DrinkCreateVm
            {
                Id = drink.Id,
                Name = drink.Name,
                Price = drink.Price,
                CategoryId = drink.CategoryId,
                Description = drink.Description,
                ImageUrl = drink.ImageUrl,
                IsActive = drink.IsActive,
                IsFeatured = drink.IsFeatured
            };

            await LoadCategoriesAsync(drink.CategoryId);
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DrinkCreateVm vm)
        {
            if (id != vm.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(vm.CategoryId);
                return View(vm);
            }

            var drink = await _db.Drinks.FindAsync(id);
            if (drink == null) return NotFound();

            drink.Name = vm.Name.Trim();
            drink.Price = vm.Price;
            drink.CategoryId = vm.CategoryId;
            drink.Description = vm.Description;
            drink.IsActive = vm.IsActive;
            drink.IsFeatured = vm.IsFeatured;

            // Nếu có ảnh mới → cập nhật
            if (vm.ImageFile != null)
            {
                TryDeleteImage(drink.ImageUrl);

                var url = await SaveImageAsync(vm.ImageFile);
                if (url == null)
                {
                    ModelState.AddModelError("ImageFile", "Ảnh không hợp lệ!");
                    await LoadCategoriesAsync(vm.CategoryId);
                    return View(vm);
                }

                drink.ImageUrl = url;
            }

            await _db.SaveChangesAsync();

            TempData["ok"] = "Đã cập nhật đồ uống.";
            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE =================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var drink = await _db.Drinks
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (drink == null) return NotFound();
            return View(drink);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var drink = await _db.Drinks.FindAsync(id);
            if (drink != null)
            {
                TryDeleteImage(drink.ImageUrl);
                _db.Drinks.Remove(drink);
                await _db.SaveChangesAsync();
            }

            TempData["ok"] = "Đã xoá đồ uống.";
            return RedirectToAction(nameof(Index));
        }

        // ================= HELPERS =================
        private async Task LoadCategoriesAsync(int? selectedId = null)
        {
            ViewBag.Categories = new SelectList(
                await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(),
                "Id", "Name", selectedId
            );
        }

        private async Task<string?> SaveImageAsync(IFormFile file)
        {
            var allow = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allow.Contains(ext)) return null;

            var folder = Path.Combine(_env.WebRootPath, "img", "drinks");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/img/drinks/{fileName}";
        }

        private void TryDeleteImage(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;
            if (!url.StartsWith("/img/")) return;

            var path = Path.Combine(_env.WebRootPath, url.TrimStart('/'));
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }
    }
}
