using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FreshDrink.Data;

namespace FreshDrink.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrinksController : ControllerBase
    {
        private readonly FreshDrinkDbContext _db;
        private readonly IConfiguration _config;

        public DrinksController(FreshDrinkDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var baseUrl = _config["App:BaseUrl"] ?? "http://localhost:5005";

            var drinks = await _db.Drinks
                .Include(d => d.Category)
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    Price = $"{d.Price:N0}đ",
                    Category = d.Category != null ? d.Category.Name : null,
                    Image = $"{baseUrl}{d.ImageUrl}"
                })
                .ToListAsync();

            return Ok(drinks);
        }
    }
}
