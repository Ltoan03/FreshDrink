using Microsoft.AspNetCore.Identity;

namespace FreshDrink.Data.Identity
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }  // 👈 Thêm dòng này
    }
}
