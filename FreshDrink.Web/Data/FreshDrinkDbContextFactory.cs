using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FreshDrink.Data
{
    public class FreshDrinkDbContextFactory : IDesignTimeDbContextFactory<FreshDrinkDbContext>
    {
        public FreshDrinkDbContext CreateDbContext(string[] args)
        {
            var cfg = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var opts = new DbContextOptionsBuilder<FreshDrinkDbContext>()
                .UseSqlServer(cfg.GetConnectionString("DefaultConnection"))
                .Options;

            return new FreshDrinkDbContext(opts);
        }
    }
}
