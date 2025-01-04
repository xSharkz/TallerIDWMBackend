using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TallerIDWMBackend.Data;
using TallerIDWMBackend.Helpers;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.Repository;
using TallerIDWMBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Cargar variables de entorno
Env.Load();

// Configuración CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:4200") // Cambia esto al origen de tu cliente
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); // Usar solo si tus requests implican credenciales
    });
});

// Swagger y Controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true; // Permitir Accept Headers
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Desactivar la convención camelCase en JSON
});

// Conexión a la base de datos
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Inyección de dependencias
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserContextService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<InvoiceService>();

// Configuración JWT
var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
if (string.IsNullOrEmpty(jwtSecretKey))
{
    throw new InvalidOperationException("JWT_SECRET_KEY no está configurado.");
}
var key = Encoding.ASCII.GetBytes(jwtSecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Leer el token de la cookie en lugar del header
            var token = context.Request.Cookies["jwt_token"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? throw new InvalidOperationException("JWT_SECRET_KEY not found")
        )),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Sembrar datos (Data Seeder)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seeder = services.GetRequiredService<DataSeeder>();
    await seeder.SeedProductsAsync();
    await seeder.SeedAdminUserAsync();
}

// Middleware para diagnóstico de rutas
app.Use(async (context, next) =>
{
    Console.WriteLine($"Ruta solicitada: {context.Request.Path}");
    await next();
});

// Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
app.UseRouting();
app.UseHttpsRedirection();
app.UseCors("MyCorsPolicy"); // Asegúrate de que CORS se aplique antes de la autenticación
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();