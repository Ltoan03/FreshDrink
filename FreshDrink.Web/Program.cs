using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using FreshDrink.Data;
using FreshDrink.Data.Identity;
using FreshDrink.Web.Data;


var builder = WebApplication.CreateBuilder(args);

// ----------------------- DATABASE -----------------------
builder.Services.AddDbContext<AuthDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<FreshDrinkDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// ----------------------- IDENTITY -----------------------
builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.SignIn.RequireConfirmedAccount = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = true;
    opt.Password.RequiredLength = 4;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();


// ----------------------- AUTH COOKIE -----------------------
builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.LoginPath = "/Account/Login";
    opt.LogoutPath = "/Account/Logout";
    opt.AccessDeniedPath = "/Account/AccessDenied";
    opt.SlidingExpiration = true;
    opt.ExpireTimeSpan = TimeSpan.FromDays(14);

    opt.Cookie.Name = "FreshDrink.Auth";
    opt.Cookie.HttpOnly = true;
    opt.Cookie.SameSite = SameSiteMode.Lax;
});


// ----------------------- AUTHORIZATION -----------------------
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
});


// ----------------------- LOCALIZATION -----------------------
var supportedCultures = new[] { new CultureInfo("vi-VN") };
builder.Services.Configure<RequestLocalizationOptions>(o =>
{
    o.DefaultRequestCulture = new RequestCulture("vi-VN");
    o.SupportedCultures = supportedCultures;
    o.SupportedUICultures = supportedCultures;
});


// ----------------------- MVC / TempData -----------------------
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();
builder.Services.AddRazorPages();


// ----------------------- SESSION -----------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromHours(6);
    opt.Cookie.Name = "FreshDrink.Session";
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
    opt.Cookie.SameSite = SameSiteMode.Lax;
});


// ----------------------- SERVICES -----------------------
builder.Services.AddTransient<IdentitySeeder>(); // Admin seed


var app = builder.Build();


// ----------------------- RUN SEEDERS -----------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FreshDrinkDbContext>();

    // 1) Seed data mặc định (Drinks, Categories,...)
    await DataSeeder.SeedAsync(app);

    // 2) Tạo tài khoản admin nếu chưa có
    var identitySeeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
    await identitySeeder.SeedAsync();

    // ⚠️ 3) Tạo file seeder từ database (DÙNG LÚC CẬP NHẬT DỮ LIỆU → rồi xoá dòng này)
    // await SeederGenerator.GenerateDrinkSeederAsync(db);
}


// ----------------------- PIPELINE -----------------------
app.UseRequestLocalization(app.Services
    .GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


// ----------------------- ROUTES -----------------------
app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Drinks}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Store}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();
await DataSeeder.SeedAsync(app);
