using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FreshDrink.Data.Identity;

namespace FreshDrink.Data.Models
{
    public enum OrderStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        // 🔥 Navigation đúng chuẩn Identity FK
        public AppUser? User { get; set; }

        // EF sẽ tự tạo default
        public DateTime CreatedAt { get; set; }

        // enum, không đánh MaxLength ở đây
        public OrderStatus Status { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
