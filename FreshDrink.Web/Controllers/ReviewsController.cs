using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FreshDrink.Data;
using FreshDrink.Data.Models;
using System.Security.Claims;

namespace FreshDrink.Web.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly FreshDrinkDbContext _db;

        public ReviewsController(FreshDrinkDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int drinkId, int rating, string? comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            // Validate rating
            if (rating is < 1 or > 5)
            {
                TempData["error"] = "⚠ Vui lòng chọn mức đánh giá hợp lệ!";
                return Redirect($"/do-uong/{drinkId}");
            }

            // Check if drink exists
            var drink = await _db.Drinks.FirstOrDefaultAsync(x => x.Id == drinkId);
            if (drink == null)
            {
                TempData["error"] = "⚠ Sản phẩm không tồn tại!";
                return Redirect("/do-uong");
            }

            // Prevent duplicate review from same user
            bool exists = await _db.Reviews.AnyAsync(r => r.UserId == userId && r.DrinkId == drinkId);
            if (exists)
            {
                TempData["error"] = "⚠ Bạn đã đánh giá sản phẩm này rồi!";
                return Redirect($"/do-uong/{drinkId}");
            }

            // Create review
            var review = new Review
            {
                DrinkId = drinkId,
                UserId = userId,
                Rating = rating,
                Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim(),
                CreatedAt = DateTime.Now,
            };

            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            // Recalculate rating (safe cast)
            drink.Rating = (decimal)await _db.Reviews
                .Where(r => r.DrinkId == drinkId)
                .AverageAsync(r => r.Rating);

            // Update review count
            drink.ReviewsCount = await _db.Reviews.CountAsync(r => r.DrinkId == drinkId);

            await _db.SaveChangesAsync();

            TempData["ok"] = "🎉 Cảm ơn bạn đã gửi đánh giá!";
            return Redirect($"/do-uong/{drinkId}");
        }
    }
}
