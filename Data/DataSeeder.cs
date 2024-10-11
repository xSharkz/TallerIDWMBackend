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
                .FinishWith((f, p) => Console.WriteLine($"Producto generado: {p.Name}, Precio: {p.Price}"));

            var products = faker.Generate(10);

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
        }

        public async Task SeedPostsAsync()
        {
            if (await _context.Posts.AnyAsync()) return;

            var faker = new Faker<Post>()
                .RuleFor(p => p.Title, f => f.Lorem.Sentence())
                .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
                .RuleFor(p => p.Url, f => f.Internet.Url())
                .RuleFor(p => p.PublicId, f => Guid.NewGuid().ToString())
                .FinishWith((f, p) => Console.WriteLine($"Post generado: {p.Title}, URL: {p.Url}"));

            var posts = faker.Generate(5);

            await _context.Posts.AddRangeAsync(posts);
            await _context.SaveChangesAsync();
        }

    }
}