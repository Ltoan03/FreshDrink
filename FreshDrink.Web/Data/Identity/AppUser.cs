using Microsoft.AspNetCore.Identity;

namespace FreshDrink.Web.Data.Identity
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
