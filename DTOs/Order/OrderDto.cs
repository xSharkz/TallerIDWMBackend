using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.Order
{
    /// <summary>
    /// Representa un pedido realizado por un usuario.
    /// </summary>
    public class OrderDto
    {
        /// <summary>
        /// Identificador único del pedido.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Identificador único del usuario que realizó el pedido.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y vincula el pedido con el usuario correspondiente.
        /// </remarks>
        [Required]
        public long UserId { get; set; }

        /// <summary>
        /// Fecha en la que se realizó el pedido.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio.
        /// </remarks>
        [Required]
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Monto total del pedido.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe tener un valor entre 0 y 100,000,000.
        /// </remarks>
        [Required]
        [Range(0, 100000000)]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Dirección de entrega del pedido.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y puede tener un máximo de 255 caracteres.
        /// </remarks>
        [Required]
        [StringLength(255)]
        public string DeliveryAddress { get; set; } = null!;

        /// <summary>
        /// Lista de elementos incluidos en el pedido.
        /// </summary>
        /// <remarks>
        /// Esta lista contiene los detalles de cada producto incluido en el pedido.
        /// </remarks>
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}
