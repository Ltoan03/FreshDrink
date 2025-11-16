using System.Collections.Generic;
using System.Linq;

namespace FreshDrink.Web.ViewModels
{
    public class CartVm
    {
        public List<CartItemVm> Items { get; set; } = new();

        // ==== CALCULATIONS ====
        public int TotalQuantity => Items.Sum(x => x.Quantity);

        public decimal Subtotal => Items.Sum(x => x.LineTotal);

        public decimal Shipping { get; set; } = 0m;

        // 👉 số tiền giảm sau khi dùng Voucher
        public decimal DiscountAmount { get; set; } = 0m;

        // 👉 mã voucher đã áp dụng
        public string? VoucherCode { get; set; }

        // 👉 tổng sau khi tính giảm giá + ship
        public decimal Total => Subtotal + Shipping - DiscountAmount;
    }
}
