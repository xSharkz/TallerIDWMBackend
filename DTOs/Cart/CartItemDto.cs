using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.Cart
{
    /// <summary>
    /// Representa un elemento en el carrito de compras.
    /// </summary>
    public class CartItemDto
    {
        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y representa la relación con el producto correspondiente.
        /// </remarks>
        [Required]
        public long ProductId { get; set; }

        /// <summary>
        /// Cantidad del producto en el carrito.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe tener un valor entre 1 y 1000.
        /// </remarks>
        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }

        /// <summary>
        /// Identificador de sesión del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo puede ser utilizado para vincular el carrito con una sesión específica del usuario.
        /// </remarks>
        public string SessionId { get; set; } = null!;
    }
}
