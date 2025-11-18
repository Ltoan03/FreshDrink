using Microsoft.EntityFrameworkCore;
using FreshDrink.Data;
using FreshDrink.Web.Data;
using FreshDrink.Data.Identity;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext
builder.Services.AddDbContext<FreshDrinkDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<FreshDrinkDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();   // IMPORTANT
app.UseAuthorization();
app.MapControllers();

// Run Seeder
await DataSeeder.SeedAsync(app);

app.Run();
