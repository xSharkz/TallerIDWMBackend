using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace TallerIDWMBackend.Models
{
    public class OrderItem
    {
        public long Id { get; set; }
        public long OrderId { get; set; } // Referencia al pedido.
        public long ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; } // Cantidad comprada.
        public decimal UnitPrice { get; set; } // Precio unitario al momento de la compra.
        public Order Order { get; set; } = null!;
    }
}