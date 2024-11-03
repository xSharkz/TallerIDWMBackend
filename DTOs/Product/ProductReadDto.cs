using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.Product
{
    public class ProductReadDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; } = null!;
    }
}