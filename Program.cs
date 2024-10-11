using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using TallerIDWMBackend.Data;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.Repository;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

//Servicios
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddScoped<DataSeeder>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seeder = services.GetRequiredService<DataSeeder>();
    await seeder.SeedProductsAsync();
    await seeder.SeedPostsAsync();
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
