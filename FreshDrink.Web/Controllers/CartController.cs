using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FreshDrink.Data;
using FreshDrink.Data.Models;
using FreshDrink.Web.ViewModels;
using FreshDrink.Web.Infrastructure;
using System.Security.Claims;

namespace FreshDrink.Web.Controllers
{
    public class CartController : Controller
    {
        private const string CartKey = "CART_ITEMS";
        private const string VoucherKey = "VOUCHER_CODE";
        private readonly FreshDrinkDbContext _db;

        public CartController(FreshDrinkDbContext db)
        {
            _db = db;
        }

        private class CartItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string? ImageUrl { get; set; }
            public decimal Price { get; set; }
            public int Qty { get; set; }
        }

        private List<CartItem> GetCart()
            => HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new();

        private void SaveCart(List<CartItem> items)
        {
            HttpContext.Session.SetObject(CartKey, items);

            // 👉 Cập nhật số lượng trong session để hiển thị badge
            HttpContext.Session.SetInt32("CartCount", items.Sum(x => x.Qty));
        }


        // ===== APPLY VOUCHER =====
        [HttpGet]
        public async Task<IActionResult> ApplyVoucher(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Json(new { success = false, message = "⚠ Vui lòng nhập mã voucher!" });

            var voucher = await _db.Vouchers.FirstOrDefaultAsync(v =>
                v.Code == code && v.IsActive && v.ExpiryDate >= DateTime.Now);

            if (voucher == null)
                return Json(new { success = false, message = "❌ Mã voucher không tồn tại hoặc đã hết hạn!" });

            HttpContext.Session.SetString(VoucherKey, voucher.Code);

            return Json(new { success = true, message = $"🎉 Áp dụng thành công: {voucher.Code}" });
        }

        [HttpPost]
        public IActionResult RemoveVoucher()
        {
            HttpContext.Session.Remove(VoucherKey);
            return RedirectToAction(nameof(Index));
        }



        // ===== ADD TO CART =====
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int drinkId, int qty = 1)
        {
            var drink = await _db.Drinks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == drinkId);
            if (drink == null) return NotFound();

            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.Id == drinkId);

            if (item == null)
                cart.Add(new CartItem { Id = drink.Id, Name = drink.Name, ImageUrl = drink.ImageUrl, Price = drink.Price, Qty = qty });
            else
                item.Qty += qty;

            SaveCart(cart);

            TempData["ok"] = "🛒 Sản phẩm đã được thêm vào giỏ hàng!";

            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }



        // ===== SHOW CART =====
        public async Task<IActionResult> Index()
        {
            var cart = GetCart();
            decimal subtotal = cart.Sum(x => x.Price * x.Qty);
            decimal discountAmount = 0;

            string? voucherCode = HttpContext.Session.GetString(VoucherKey);

            if (!string.IsNullOrEmpty(voucherCode))
            {
                var voucher = await _db.Vouchers.FirstOrDefaultAsync(v =>
                    v.Code == voucherCode && v.IsActive && v.ExpiryDate >= DateTime.Now);

                if (voucher != null)
                {
                    discountAmount = (subtotal * voucher.Discount) / 100;

                    if (voucher.MaxDiscountAmount > 0)
                        discountAmount = Math.Min(discountAmount, voucher.MaxDiscountAmount);
                }
                else
                {
                    HttpContext.Session.Remove(VoucherKey);
                }
            }

            return View(new CartVm
            {
                Items = cart.Select(x => new CartItemVm
                {
                    DrinkId = x.Id,
                    Name = x.Name,
                    ImageUrl = x.ImageUrl,
                    Quantity = x.Qty,
                    UnitPrice = x.Price
                }).ToList(),

                VoucherCode = voucherCode,
                DiscountAmount = discountAmount
            });
        }



        // ===== UPDATE QUANTITY =====
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Update(int drinkId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.Id == drinkId);

            if (item != null)
            {
                item.Qty = Math.Max(1, quantity);
                SaveCart(cart);
            }

            return RedirectToAction(nameof(Index));
        }



        // ===== REMOVE ITEM =====
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Remove(int drinkId)
        {
            var cart = GetCart();
            cart.RemoveAll(x => x.Id == drinkId);
            SaveCart(cart);

            return RedirectToAction(nameof(Index));
        }


        // ===== CLEAR CART =====
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CartKey);
            HttpContext.Session.Remove(VoucherKey);
            return RedirectToAction(nameof(Index));
        }


        // ===== CHECKOUT =====
        [Authorize]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Challenge();

            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["error"] = "⚠ Giỏ hàng trống!";
                return RedirectToAction(nameof(Index));
            }

            decimal subtotal = cart.Sum(x => x.Price * x.Qty);
            decimal discountAmount = 0;
            string? voucherCode = HttpContext.Session.GetString(VoucherKey);

            if (voucherCode != null)
            {
                var voucher = await _db.Vouchers.FirstOrDefaultAsync(x =>
                    x.Code == voucherCode && x.IsActive && x.ExpiryDate >= DateTime.Now);

                if (voucher != null)
                {
                    discountAmount = (subtotal * voucher.Discount) / 100;
                    if (voucher.MaxDiscountAmount > 0)
                        discountAmount = Math.Min(discountAmount, voucher.MaxDiscountAmount);
                }
            }

            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.Now,
                Status = OrderStatus.Pending,
                Total = subtotal - discountAmount
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            _db.OrderItems.AddRange(cart.Select(c => new OrderItem
            {
                OrderId = order.Id,
                DrinkId = c.Id,
                Name = c.Name,
                UnitPrice = c.Price,
                Quantity = c.Qty,
                LineTotal = c.Price * c.Qty
            }));

            await _db.SaveChangesAsync();

            HttpContext.Session.Remove(CartKey);
            HttpContext.Session.Remove(VoucherKey);

            TempData["ok"] = "🎉 Đặt hàng thành công!";
            return RedirectToAction("Index", "Orders");
        }
    }
}
