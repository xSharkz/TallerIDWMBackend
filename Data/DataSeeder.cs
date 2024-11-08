using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TallerIDWMBackend.Models;
using Bogus;

namespace TallerIDWMBackend.Data
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;

        public DataSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedProductsAsync()
        {
            if (await _context.Products.AnyAsync()) return;

            var faker = new Faker<Product>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Type, f => f.PickRandom(new[] { "Poleras", "Gorros", "Juguetería", "Alimentación", "Libros" }))
                .RuleFor(p => p.Price, f => f.Random.Int(0, 99999999))
                .RuleFor(p => p.StockQuantity, f => f.Random.Int(0, 99999))
                .RuleFor(p => p.ImageUrl, f => f.Internet.Avatar())
                .RuleFor(p => p.PublicId, f => Guid.NewGuid().ToString())  // Asignar un valor a PublicId
                .FinishWith((f, p) => Console.WriteLine($"Producto generado: {p.Name}, Precio: {p.Price}"));


            var products = faker.Generate(10);

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
        }
        public async Task SeedAdminUserAsync()
        {
            if (await _context.Users.AnyAsync()) return;

            // Crear el administrador
            var adminUser = new User
            {
                Rut = "20.416.699-4",
                Name = "Ignacio Mancilla",
                BirthDate = new DateTime(2000, 10, 25),
                Email = "admin@idwm.cl",
                Gender = "Masculino",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("P4ssw0rd"), // Encriptar la contraseña
                IsAdmin = true,
                IsEnabled = true
            };
            // Crear el administrador
            var studentUser = new User
            {
                Rut = "21.121.912-2",
                Name = "Martin Becerra",
                BirthDate = new DateTime(2002, 09, 04),
                Email = "martin@idwm.cl",
                Gender = "Masculino",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("martin123"), // Encriptar la contraseña
                IsAdmin = false,
                IsEnabled = true
            };

            _context.Users.Add(adminUser);
            _context.Users.Add(studentUser);
            await _context.SaveChangesAsync();

            Console.WriteLine("Administrador creado: Ignacio Mancilla (admin@idwm.cl)");
        }

    }
}