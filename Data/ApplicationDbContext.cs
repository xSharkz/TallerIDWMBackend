using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TallerIDWMBackend.Models;

namespace TallerIDWMBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){

        }

        public DbSet<Product> Products { get; set;}

        public DbSet<Post> Posts { get; set;}

        public DbSet<CartItem> CartItems { get; set;}

        public DbSet<Order> Orders { get; set;}

        public DbSet<OrderItem> OrderItems { get; set;}

        public DbSet<User> Users { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar relación de CartItem con Product
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Product)
                .WithMany(p => p.CartItems) 
                .HasForeignKey(c => c.ProductId);

            // Configurar relación de OrderItem con Product
            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.Product)
                .WithMany(p => p.OrderItems) 
                .HasForeignKey(o => o.ProductId);
        }
    }
}