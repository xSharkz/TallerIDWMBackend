using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TallerIDWMBackend.Models
{
    public class Order
    {
        public long Id { get; set; }
        public long UserId { get; set; } // Referencia al cliente que hizo el pedido.
        public DateTime OrderDate { get; set; } // Fecha del pedido.
        public decimal TotalAmount { get; set; } // Total a pagar.
        public string DeliveryAddress { get; set; } = null!; // Dirección de entrega.
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}