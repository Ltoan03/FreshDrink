using Microsoft.AspNetCore.Mvc;
using FreshDrink.Api.Data;
using FreshDrink.Api.Models;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Category>> Get() =>
        await db.Categories.AsNoTracking().ToListAsync();

    [HttpPost]
    public async Task<IActionResult> Create(Category c)
    {
        db.Categories.Add(c);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = c.Id }, c);
    }
}
