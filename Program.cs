using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TallerIDWMBackend.Data;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.Repository;

var builder = WebApplication.CreateBuilder(args);

// Cargar variables de entorno
Env.Load();

// Swagger y Controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Conexión a la base de datos
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Inyección de dependencias
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Configuración JWT
var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY")!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // Opcional: puedes configurarlo si tienes un emisor específico.
        ValidateAudience = false, // Opcional: puedes configurarlo si tienes una audiencia específica.
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Elimina retrasos de expiración del token.
    };
});

var app = builder.Build();

// Sembrar datos (Data Seeder)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seeder = services.GetRequiredService<DataSeeder>();
    await seeder.SeedProductsAsync();
    await seeder.SeedPostsAsync();
}

// Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
app.UseHttpsRedirection();
app.UseAuthentication(); // Asegúrate de añadir esta línea para usar JWT
app.UseAuthorization();
app.MapControllers();
app.Run();