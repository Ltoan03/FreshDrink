using System.ComponentModel.DataAnnotations;

namespace FreshDrink.Web.ViewModels
{
    public class RegisterVm
    {
        [Required, EmailAddress, Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Xác nhận mật khẩu")]
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }
}
