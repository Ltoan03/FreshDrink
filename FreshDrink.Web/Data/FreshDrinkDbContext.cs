using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using FreshDrink.Data.Models;
using FreshDrink.Data.Identity;

namespace FreshDrink.Data
{
    public class FreshDrinkDbContext : IdentityDbContext<AppUser>
    {
        public FreshDrinkDbContext(DbContextOptions<FreshDrinkDbContext> options) : base(options) { }

        // ================== TABLES ==================
        public DbSet<Drink> Drinks => Set<Drink>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Voucher> Vouchers => Set<Voucher>();
        public DbSet<Review> Reviews => Set<Review>(); // 👈 review table


        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // ---------- Drink ----------
            b.Entity<Drink>(e =>
            {
                e.Property(p => p.Name).HasMaxLength(200).IsRequired();
                e.Property(p => p.Price).HasColumnType("decimal(18,2)");

                // NEW: Rating + ReviewCount mapping
                e.Property(p => p.Rating)
                 .HasColumnType("decimal(3,2)")
                 .HasDefaultValue(0);

                e.Property(p => p.ReviewsCount)
                 .HasDefaultValue(0);
            });

            // ---------- Category ----------
            b.Entity<Category>(e =>
            {
                e.Property(p => p.Name).HasMaxLength(200).IsRequired();

                e.HasData(
                    new Category { Id = 1, Name = "Tea", IsActive = true },
                    new Category { Id = 2, Name = "Coffee", IsActive = true },
                    new Category { Id = 3, Name = "Milk Tea", IsActive = true },
                    new Category { Id = 4, Name = "Smoothie", IsActive = true }
                );
            });

            // ---------- Order ----------
            b.Entity<Order>(e =>
            {
                e.Property(o => o.Status)
                 .HasConversion<string>()
                 .HasMaxLength(50);

                e.Property(o => o.Total).HasColumnType("decimal(18,2)");

                e.HasOne<AppUser>()
                 .WithMany()
                 .HasForeignKey(o => o.UserId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------- OrderItem ----------
            b.Entity<OrderItem>(e =>
            {
                e.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
                e.Property(i => i.LineTotal).HasColumnType("decimal(18,2)");

                e.HasOne(i => i.Order)
                 .WithMany(o => o.Items)
                 .HasForeignKey(i => i.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------- Voucher ----------
            b.Entity<Voucher>(e =>
            {
                e.Property(v => v.Code).HasMaxLength(50).IsRequired();
                e.Property(v => v.MaxDiscountAmount).HasColumnType("decimal(18,2)");
            });

            // ---------- Review ----------
            b.Entity<Review>(e =>
            {
                e.Property(r => r.Comment).HasMaxLength(500);
                e.Property(r => r.Rating).IsRequired();

                e.HasOne(r => r.Drink)
                 .WithMany(d => d.Reviews)
                 .HasForeignKey(r => r.DrinkId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne<AppUser>()
                 .WithMany()
                 .HasForeignKey(r => r.UserId)
                 .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
