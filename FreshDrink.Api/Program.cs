var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();   // <-- có sau khi cài Swashbuckle

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();               // <-- có sau khi cài Swashbuckle
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
