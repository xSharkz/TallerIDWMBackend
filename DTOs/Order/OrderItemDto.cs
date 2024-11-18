using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.Order
{
    public class OrderItemDto
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public long ProductId { get; set; }
        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }
        [Required]
        [Range(1, 100000000)]
        public decimal UnitPrice { get; set; }
    }
}