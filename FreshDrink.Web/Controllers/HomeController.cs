// HomeController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FreshDrink.Data;
using FreshDrink.Web.Models;

public class HomeController : Controller
{
    private readonly FreshDrinkDbContext _db;

    public HomeController(FreshDrinkDbContext db) 
        => _db = db;

    // 👉 đảm bảo "/" luôn vào trang chủ
    [HttpGet("/")]
    [HttpGet("/home")]
    [HttpGet("/home/index")]
    public async Task<IActionResult> Index()
    {
        // Banner (2 ảnh bạn đã để trong wwwroot/img)
        ViewBag.Banners = new[]
        {
            "/img/Fresh Drink.svg",
            "/img/Fresh Drink (1).svg",
        };

        // 👉 Lấy 12 sản phẩm mới nhất
        var featured = await _db.Drinks
            .AsNoTracking()
            .Where(d => d.IsActive)
            .OrderByDescending(d => d.Id)
            .Take(12)
            .Select(d => new DrinkListItem
            {
                Id = d.Id,
                Name = d.Name,
                Price = d.Price,
                ImageUrl = d.ImageUrl,
                CategoryName = d.Category != null ? d.Category.Name : "",
                IsActive = d.IsActive
            })
            .ToListAsync();

        return View(featured);
    }
}
