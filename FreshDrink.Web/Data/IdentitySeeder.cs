using Microsoft.AspNetCore.Identity;
using FreshDrink.Data.Identity;
using System.Threading.Tasks;

namespace FreshDrink.Web.Data
{
    public class IdentitySeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public IdentitySeeder(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            // 1️⃣ Tạo role
            var roles = new[] { "Admin", "User" };
            foreach (var role in roles)
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

            // 2️⃣ Tạo tài khoản admin
            string adminEmail = "admin@freshdrink.local";
            string adminUser = "admin";
            string adminPass = "Admin@123";

            var admin = await _userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new AppUser
                {
                    UserName = adminUser,
                    Email = adminEmail,
                    FullName = "Administrator",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(admin, adminPass);
                if (result.Succeeded)
                    await _userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
