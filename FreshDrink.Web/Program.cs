using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; // CreateScope
using FreshDrink.Web.Data;
using FreshDrink.Web.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Db cho Identity (+ retry cho lỗi kết nối tạm thời)
builder.Services.AddDbContext<AuthDbContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("AuthConnection"),
        sql => sql.EnableRetryOnFailure()
    )
);

// Identity + Cookie
builder.Services.AddIdentity<AppUser, IdentityRole>(o =>
{
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequireUppercase = false;
    o.Password.RequireDigit = false;
    o.Password.RequiredLength = 4;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.LoginPath = "/Account/Login";
    opt.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

/* Storefront: /store */
app.MapControllerRoute(
    name: "store",
    pattern: "store/{action=Index}/{id?}",
    defaults: new { controller = "Store" });

/* Admin (mặc định): / */
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

/* ===== Auto-migrate + seed (bỏ qua khi chạy tool design-time) ===== */
var isDesignTime = AppDomain.CurrentDomain
    .GetAssemblies()
    .Any(a => a.GetName().Name == "Microsoft.EntityFrameworkCore.Design");

if (!isDesignTime)
{
    using var scope = app.Services.CreateScope();
    var sp = scope.ServiceProvider;

    // Tạo DB + áp migrations nếu chưa có
    var db = sp.GetRequiredService<AuthDbContext>();
    await db.Database.MigrateAsync();

    // Seed role + admin
    var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = sp.GetRequiredService<UserManager<AppUser>>();

    foreach (var r in new[] { "admin", "user" })
        if (!await roleMgr.RoleExistsAsync(r))
            await roleMgr.CreateAsync(new IdentityRole(r));

    var admin = await userMgr.FindByEmailAsync("admin@freshdrink.local");
    if (admin is null)
    {
        admin = new AppUser
        {
            UserName = "admin@freshdrink.local",
            Email = "admin@freshdrink.local",
            FullName = "Administrator"
        };
        await userMgr.CreateAsync(admin, "1234"); // đổi khi nộp
        await userMgr.AddToRoleAsync(admin, "admin");
    }
}
/* ===== End auto-migrate + seed ===== */

app.Run();
