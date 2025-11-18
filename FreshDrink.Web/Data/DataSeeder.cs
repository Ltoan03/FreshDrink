using FreshDrink.Data;
using FreshDrink.Data.Models;
using FreshDrink.Data.Identity;
using Microsoft.AspNetCore.Identity;

namespace FreshDrink.Web.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FreshDrinkDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // ==================== 1) Seed Role =======================
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            // ==================== 2) Seed Admin User ==================
            var adminEmail = "admin@freshdrink.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    FullName = "FreshDrink Administrator",
                    Email = adminEmail,
                    UserName = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // ==================== 3) Seed Categories ==================
            var categoryNames = new[] { "Coconut", "Coffee", "Juice", "Milk Tea", "Smoothie", "Soda", "Tea" };

            foreach (var name in categoryNames)
            {
                if (!db.Categories.Any(c => c.Name == name))
                {
                    db.Categories.Add(new Category { Name = name, IsActive = true });
                }
            }
            db.SaveChanges();

            // ==================== 4) Seed Drinks =====================
            void AddDrink(string name, decimal price, string category, string img)
            {
                if (!db.Drinks.Any(d => d.Name == name))
                {
                    var cat = db.Categories.FirstOrDefault(c => c.Name == category);
                    if (cat != null)
                    {
                        db.Drinks.Add(new Drink
                        {
                            Name = name,
                            Price = price,
                            CategoryId = cat.Id,
                            ImageUrl = img,
                            IsActive = true
                        });
                    }
                }
            }

            // Your existing seed list
            AddDrink("Trà Sữa Trân Châu Đường Đen", 25000, "Coffee", "/img/drinks/tra-sua-tran-chau-duong-den.jpg");
            AddDrink("Caffee Sữa", 25000, "Coffee", "/img/drinks/tra-vai.jpg");
            AddDrink("Coffee Đen", 20000, "Coffee", "/img/drinks/2.jpg");
            AddDrink("Bạc xỉu", 28000, "Coffee", "/img/drinks/3.jpg");
            AddDrink("Dừa Xiêm Dỡ", 30000, "Coconut", "/img/drinks/4.jpg");
            AddDrink("Dứa tươi", 25000, "Juice", "/img/drinks/5.jpg");
            AddDrink("Matchalate", 35000, "Milk Tea", "/img/drinks/6.jpg");
            AddDrink("Nuốc Cam", 30000, "Juice", "/img/drinks/7.jpg");
            AddDrink("Nước Ép Dủ Hấu", 35000, "Juice", "/img/drinks/8.jpg");
            AddDrink("Nước Ép Thơm", 28000, "Juice", "/img/drinks/9.png");
            AddDrink("Nước Chanh", 25000, "Juice", "/img/drinks/10.webp");
            AddDrink("Sinh Tố Bơ", 40000, "Smoothie", "/img/drinks/11.jpg");
            AddDrink("Sinh Tố Dâu", 40000, "Smoothie", "/img/drinks/12.jpg");
            AddDrink("Sinh Tố Xoài", 35000, "Smoothie", "/img/drinks/13.jpg");
            AddDrink("Soda Dâu", 25000, "Soda", "/img/drinks/14.jpg");
            AddDrink("Soda Việt Quốc", 25000, "Soda", "/img/drinks/15.jpg");
            AddDrink("Trà Lipton Nóng", 35000, "Tea", "/img/drinks/16.jpg");
            AddDrink("Trà Sữa Khoai Môn", 45000, "Milk Tea", "/img/drinks/17.jpg");
            AddDrink("Trà Sữa Socola", 45000, "Milk Tea", "/img/drinks/18.jpg");
            AddDrink("Trà Sữa Truyền Thống", 35000, "Milk Tea", "/img/drinks/19.jpg");
            AddDrink("Trà Vải", 30000, "Tea", "/img/drinks/bacxiu.png");

            db.SaveChanges();
        }
    }
}
