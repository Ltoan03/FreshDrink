using System.ComponentModel.DataAnnotations;

namespace FreshDrink.Web.ViewModels
{
    public class LoginVm
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Ghi nhớ")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
