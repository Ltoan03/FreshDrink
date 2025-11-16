using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshDrink.Data.Models
{
    public class Voucher
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã voucher không được để trống.")]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        // % giảm giá
        [Range(1, 100, ErrorMessage = "Giảm giá phải từ 1% đến 100%.")]
        public int Discount { get; set; }

        // Giảm tối đa (VNĐ) nếu áp dụng % ⇒ giới hạn số tiền giảm
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxDiscountAmount { get; set; } = 0;

        // Ngày hết hạn
        [Required]
        public DateTime ExpiryDate { get; set; }

        // Trạng thái kích hoạt voucher
        public bool IsActive { get; set; } = true;

        // Ngày tạo voucher
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
