using Microsoft.EntityFrameworkCore;
using FreshDrink.Data;

namespace FreshDrink.Web.Startup
{
    public static class AddAppDataExtension
    {
        public static IServiceCollection AddAppData(this IServiceCollection sv, IConfiguration cfg)
        {
            sv.AddDbContext<FreshDrinkDbContext>(opt =>
                opt.UseSqlServer(cfg.GetConnectionString("DefaultConnection")));
            return sv;
        }
    }
}
