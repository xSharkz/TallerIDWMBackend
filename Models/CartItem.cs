using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace TallerIDWMBackend.Models
{
    public class CartItem
    {
        public long Id { get; set; }

        [Required]
        public long ProductId { get; set; } // Referencia al producto.

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; } // Cantidad en el carrito.

        [Required]
        public string SessionId { get; set; } = null!; // Identificador de la sesión del usuario no autenticado.

        public Product Product { get; set; } = null!;
    }
}