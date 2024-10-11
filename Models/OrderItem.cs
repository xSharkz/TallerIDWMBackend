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

        [Required]
        public long OrderId { get; set; } // Referencia al pedido.

        [Required]
        public long ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; } // Cantidad comprada.

        [Required]
        [Range(1, 100000000)]
        public decimal UnitPrice { get; set; } // Precio unitario al momento de la compra.
        public Order Order { get; set; } = null!;
    }
}