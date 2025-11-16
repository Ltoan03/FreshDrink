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
    opt.Cookie.SecurePolicy = CookieSecurePolicy.None;
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


// ----------------------- SEED ADMIN -----------------------
builder.Services.AddTransient<IdentitySeeder>();

var app = builder.Build();


// ---- Run Seeder automatically ----
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
    await seeder.SeedAsync();
}


// ----------------------- PIPELINE -----------------------
app.UseRequestLocalization(app.Services
    .GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Cookie policy fix
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax
});

// ⭐ Session trước Authentication
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
