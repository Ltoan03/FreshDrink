using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FreshDrink.Data.Identity;        // AppUser
using FreshDrink.Web.ViewModels;       // LoginVm, RegisterVm

namespace FreshDrink.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // === ĐĂNG NHẬP ===
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
            => View(new LoginVm { ReturnUrl = returnUrl });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Cho phép dùng email làm username
            var user = await _userManager.FindByEmailAsync(vm.Email);
            var userName = user?.UserName ?? vm.Email;

            var result = await _signInManager.PasswordSignInAsync(
                userName, vm.Password, vm.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
                return LocalRedirect(vm.ReturnUrl ?? Url.Content("~/"));

            ModelState.AddModelError(string.Empty, "Đăng nhập không thành công.");
            return View(vm);
        }

        // === ĐĂNG XUẤT ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect(returnUrl ?? Url.Content("~/"));
        }

        // === ĐĂNG KÝ ===
        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            return View(new RegisterVm { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = new AppUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                EmailConfirmed = true // Dev mode: bỏ xác nhận email
            };

            var result = await _userManager.CreateAsync(user, vm.Password);

            if (result.Succeeded)
            {
                // (Tùy chọn) Gán role mặc định
                // await _userManager.AddToRoleAsync(user, "User");

                // Đăng nhập luôn sau khi tạo tài khoản
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(vm.ReturnUrl ?? Url.Content("~/"));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(vm);
        }
    }
}
