using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace TallerIDWMBackend.Models
{
    public class Product
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string Type { get; set; } = null!;

        public int Price { get; set; }

        public int StockQuantity { get; set; }

        public string ImageUrl { get; set; } = null!;

        public string PublicId { get; set; } = null!; // Identificador en Cloudinary

        // Relación con CartItem
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        // Relación con OrderItem
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}