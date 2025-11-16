using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshDrink.Data.Models
{
    public class Drink
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên đồ uống không được bỏ trống")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1000, 500000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        // 👉 trạng thái sản phẩm
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;

        // ===================== THÊM CHỨC NĂNG REVIEW =====================

        // 🛒 tổng số lượt mua sản phẩm
        public int PurchaseCount { get; set; } = 0;

        // ⭐ điểm đánh giá trung bình (tính từ bảng Review)
        [Column(TypeName = "decimal(3,2)")]
        public decimal Rating { get; set; } = 0;

        // ⭐ số lượt đánh giá (để tính rating trung bình)
        public int RatingCount { get; set; } = 0;

        // liên kết với bảng Review
        public ICollection<Review>? Reviews { get; set; }

         public int ReviewsCount { get; set; } = 0;

    }
}
