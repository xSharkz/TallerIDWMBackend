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
    }
}