using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.Order
{
    public class OrderDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string DeliveryAddress { get; set; } = null!;
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}