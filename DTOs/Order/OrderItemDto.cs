using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.Order
{
    /// <summary>
    /// Representa un elemento individual dentro de un pedido.
    /// </summary>
    public class OrderItemDto
    {
        /// <summary>
        /// Identificador único del elemento del pedido.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio.
        /// </remarks>
        [Required]
        public long Id { get; set; }

        /// <summary>
        /// Identificador único del producto asociado al elemento del pedido.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y vincula el elemento del pedido con un producto específico.
        /// </remarks>
        [Required]
        public long ProductId { get; set; }

        /// <summary>
        /// Cantidad del producto solicitada en el pedido.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe estar en el rango de 1 a 1000.
        /// </remarks>
        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }

        /// <summary>
        /// Precio unitario del producto en el pedido.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe estar en el rango de 1 a 100,000,000.
        /// </remarks>
        [Required]
        [Range(1, 100000000)]
        public decimal UnitPrice { get; set; }
    }
}
