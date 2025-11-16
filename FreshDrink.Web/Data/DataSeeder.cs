using FreshDrink.Data;
using FreshDrink.Data.Models;

namespace FreshDrink.Web.Data
{
    public static class DataSeeder
    {
        public static void Seed(FreshDrinkDbContext db)
        {
            // Seed Categories nếu thiếu hoặc chưa đủ
            var categoryNames = new[] { "Coconut", "Coffee", "Juice", "Milk Tea", "Smoothie", "Soda", "Tea" };

            foreach (var name in categoryNames)
            {
                if (!db.Categories.Any(c => c.Name == name))
                {
                    db.Categories.Add(new Category { Name = name, IsActive = true });
                }
            }
            db.SaveChanges();


            // Seed Drinks (không cần check !Any, tránh skip)
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

            AddDrink("Caffee Sữa", 25000, "Coffee", "/img/drinks/88d25.jpg");
            AddDrink("Coffee Đen", 20000, "Coffee", "/img/drinks/97a8.jpg");
            AddDrink("Bạc xỉu", 28000, "Coffee", "/img/drinks/5288.png");
            AddDrink("Dừa Xiêm Dỡ", 30000, "Coconut", "/img/drinks/8c8e.webp");
            AddDrink("Dứa tươi", 25000, "Juice", "/img/drinks/38d06.jpg");
            AddDrink("Matchalate", 35000, "Milk Tea", "/img/drinks/91800.jpg");
            AddDrink("Nuốc Cam", 30000, "Juice", "/img/drinks/21a7.jpg");
            AddDrink("Nước Ép Dủ Hấu", 35000, "Juice", "/img/drinks/34b9.jpg");
            AddDrink("Nước Ép Thơm", 28000, "Juice", "/img/drinks/3673.jpg");
            AddDrink("Nước Chanh", 25000, "Juice", "/img/drinks/05a6.jpg");
            AddDrink("Sinh Tố Bơ", 40000, "Smoothie", "/img/drinks/617e.jpg");
            AddDrink("Sinh Tố Dâu", 40000, "Smoothie", "/img/drinks/7be6.jpg");
            AddDrink("Sinh Tố Xoài", 35000, "Smoothie", "/img/drinks/aae6.jpg");
            AddDrink("Soda Dâu", 25000, "Soda", "/img/drinks/e7e7.jpg");
            AddDrink("Soda Việt Quốc", 25000, "Soda", "/img/drinks/70a6.jpg");
            AddDrink("Trà Lipton Nóng", 35000, "Tea", "/img/drinks/103d.jpg");
            AddDrink("Trà Sữa Khoai Môn", 45000, "Milk Tea", "/img/drinks/d97e.jpg");
            AddDrink("Trà Sữa Socola", 45000, "Milk Tea", "/img/drinks/5d3b.jpg");
            AddDrink("Trà Sữa Truyền Thống", 35000, "Milk Tea", "/img/drinks/0df7.jpg");
            AddDrink("Trà Vải", 30000, "Tea", "/img/drinks/a79a.jpg");

            db.SaveChanges();
        }
    }
}
