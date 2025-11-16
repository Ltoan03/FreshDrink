using System;

namespace FreshDrink.Web.Models
{
    public class DrinkListItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }


        // ⭐ TRUNG BÌNH SAO (1–5)
        public double Rating { get; set; } = 0;

        // 💬 SỐ REVIEW
        public int ReviewsCount { get; set; } = 0;

        // 🛍 TỔNG LƯỢT MUA
        public int SoldCount { get; set; } = 0;
    }
}
