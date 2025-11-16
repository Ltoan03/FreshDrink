using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshDrink.Api.Models;

// ========== Người dùng & phân quyền ==========
public class AppUser
{
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required] public string Email { get; set; } = default!;
    [Required] public string PasswordHash { get; set; } = default!;
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Role
{
    [Key] public int Id { get; set; }
    [Required] public string Name { get; set; } = default!;
}

public class UserRole
{
    public string UserId { get; set; } = default!;
    public int RoleId { get; set; }
}

// ========== Danh mục & sản phẩm ==========
public class Category
{
    [Key] public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
}

public class Drink
{
    [Key] public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = default!;
    public decimal BasePrice { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ========== Giỏ hàng ==========
public class Cart
{
    [Key] public int Id { get; set; }
    public string UserId { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CartItem
{
    [Key] public int Id { get; set; }
    public int CartId { get; set; }
    public int DrinkId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}

// ========== Đơn hàng & thanh toán ==========
public class Order
{
    [Key] public int Id { get; set; }
    public string UserId { get; set; } = default!;
    public string Status { get; set; } = "Pending";
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
      public Payment? Payment { get; set; }
}

public class OrderItem
{
    [Key] public int Id { get; set; }
    public int OrderId { get; set; }
    public int DrinkId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}

public class Payment
{
    [Key] public int Id { get; set; }
    public int OrderId { get; set; }
    public string Provider { get; set; } = "COD";
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Unpaid";
    public DateTime? PaidAt { get; set; }
}
