using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using FreshDrink.Data; 

namespace FreshDrink.Web.Data
{
    public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            // Xác định thư mục gốc (nơi có appsettings.json)
            var basePath = Directory.GetCurrentDirectory();

            // Nạp file cấu hình appsettings.json
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // Lấy chuỗi kết nối
            var connStr = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
            optionsBuilder.UseSqlServer(connStr);

            return new AuthDbContext(optionsBuilder.Options);
        }
    }
}
