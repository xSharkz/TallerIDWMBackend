using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.Order
{
    public class OrderDto
    {
        public long Id { get; set; }
        [Required]
        public long UserId { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        [Range(0, 100000000)]
        public decimal TotalAmount { get; set; }
        [Required]
        [StringLength(255)]
        public string DeliveryAddress { get; set; } = null!;
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}