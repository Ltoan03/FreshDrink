using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FreshDrink.Data;
using FreshDrink.Web.Models;
using Microsoft.AspNetCore.Authorization;
using FreshDrink.Data.Models;


namespace FreshDrink.Web.Controllers
{
    [Route("do-uong")]
    public class StoreController : Controller
    {
        private readonly FreshDrinkDbContext _db;
        public StoreController(FreshDrinkDbContext db) => _db = db;

        private static string Slugify(string s)
            => s.Trim().ToLower().Replace(" ", "-");

        // ================== INDEX ==================
        [HttpGet("")]
        public async Task<IActionResult> Index(string? q, string? category, string? sort, int page = 1, int pageSize = 12)
        {
            if (page < 1) page = 1;

            var allCategories = await _db.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            int? categoryId = null;

            if (!string.IsNullOrWhiteSpace(category))
            {
                var found = allCategories.FirstOrDefault(c => Slugify(c.Name) == category.ToLower());
                if (found != null) categoryId = found.Id;
            }

            var query = _db.Drinks
                .AsNoTracking()
                .Include(d => d.Category)
                .Where(d => d.IsActive);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(d => d.Name.Contains(q.Trim()));

            if (categoryId.HasValue)
                query = query.Where(d => d.CategoryId == categoryId.Value);

            query = sort switch
            {
                "price-asc" => query.OrderBy(d => d.Price),
                "price-desc" => query.OrderByDescending(d => d.Price),
                "name" => query.OrderBy(d => d.Name),
                _ => query.OrderByDescending(d => d.Id)
            };

            var total = await query.CountAsync();

            var drinks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DrinkListItem
                {
                    Id = d.Id,
                    Name = d.Name,
                    Price = d.Price,
                    ImageUrl = d.ImageUrl,
                    CategoryName = d.Category!.Name,
                    IsActive = d.IsActive,
                    IsFeatured = d.IsFeatured,

                    Rating = _db.Reviews
                        .Where(r => r.DrinkId == d.Id)
                        .Select(r => (double)r.Rating)
                        .DefaultIfEmpty()
                        .Average(),

                    ReviewsCount = _db.Reviews.Count(r => r.DrinkId == d.Id),

                    SoldCount = _db.OrderItems
                        .Where(o => o.DrinkId == d.Id)
                        .Sum(o => (int?)o.Quantity) ?? 0
                })
                .ToListAsync();

            ViewBag.Categories = allCategories.Select(c => new
            {
                c.Id,
                c.Name,
                Slug = Slugify(c.Name)
            });

            ViewBag.PageIndex = page;
            ViewBag.HasPrev = page > 1;
            ViewBag.HasNext = page * pageSize < total;

            return View(drinks);
        }

        // ================== DETAILS ==================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var d = await _db.Drinks
                .AsNoTracking()
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

            if (d == null) return NotFound();

            var vm = new DrinkListItem
            {
                Id = d.Id,
                Name = d.Name,
                Price = d.Price,
                ImageUrl = d.ImageUrl,
                CategoryName = d.Category?.Name ?? "",
                IsActive = d.IsActive,
                IsFeatured = d.IsFeatured,

                Rating = _db.Reviews
                    .Where(r => r.DrinkId == id)
                    .Select(r => (double)r.Rating)
                    .DefaultIfEmpty()
                    .Average(),

                ReviewsCount = _db.Reviews.Count(r => r.DrinkId == id),

                SoldCount = _db.OrderItems
                    .Where(o => o.DrinkId == id)
                    .Sum(o => (int?)o.Quantity) ?? 0
            };

            // Gửi review danh sách sang UI
            ViewBag.Reviews = await _db.Reviews
                .Where(r => r.DrinkId == id)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    r.Rating,
                    r.Comment,
                    r.CreatedAt,
                    UserName = _db.Users.Where(u => u.Id == r.UserId).Select(u => u.UserName).FirstOrDefault()
                })
                .ToListAsync();

            ViewData["Title"] = vm.Name;
            return View(vm);
        }

        // ================== POST REVIEW ==================
        [HttpPost("review")]
        [Authorize] // phải đăng nhập mới gửi review
        public async Task<IActionResult> AddReview(int drinkId, int rating, string comment)
        {
            var userId = User.Identity?.Name;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var review = new Review
            {
                DrinkId = drinkId,
                Rating = rating,
                Comment = comment,
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            return Redirect($"/do-uong/{drinkId}");
        }
    }
}
