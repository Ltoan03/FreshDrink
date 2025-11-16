using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using FreshDrink.Data;
using FreshDrink.Data.Identity;

namespace FreshDrink.Web.Startup
{
    public static class AddAuthLayerExtension
    {
        public static IServiceCollection AddAuthLayer(this IServiceCollection sv, IConfiguration cfg)
        {
            // Db cho Identity
            sv.AddDbContext<AuthDbContext>(opt =>
                opt.UseSqlServer(cfg.GetConnectionString("DefaultConnection")));

            // Identity
            sv.AddDefaultIdentity<AppUser>(opt =>
            {
                opt.SignIn.RequireConfirmedAccount = false;
                // Dev: nới lỏng rule
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = true;
                opt.Password.RequiredLength = 4;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultUI();

            // Cookie
            sv.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.Cookie.Name = "FreshDrink.Auth";
            });

            // Policy
            sv.AddAuthorization(opt =>
            {
                opt.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
            });

            return sv;
        }
    }
}
