using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TallerIDWMBackend.Models
{
    /// <summary>
    /// Representa un artículo en el carrito de compras de un usuario.
    /// </summary>
    public class CartItem
    {
        /// <summary>
        /// Identificador único del artículo en el carrito.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Identificador del producto asociado a este artículo en el carrito.
        /// Se relaciona con el modelo de Producto mediante el ProductId.
        /// </summary>
        [Required]
        public long ProductId { get; set; }

        /// <summary>
        /// Producto asociado al artículo en el carrito.
        /// Este campo está enlazado al modelo <see cref="Product"/>.
        /// </summary>
        public Product Product { get; set; } = null!;

        /// <summary>
        /// Cantidad de unidades de este producto en el carrito de compras.
        /// El valor debe estar entre 1 y 1000 unidades.
        /// </summary>
        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; } // Cantidad en el carrito.

        /// <summary>
        /// Identificador único de la sesión de un usuario no autenticado.
        /// Utilizado para asociar el carrito con una sesión específica.
        /// </summary>
        [Required]
        public string SessionId { get; set; } = null!; // Identificador de la sesión del usuario no autenticado.
    }
}
