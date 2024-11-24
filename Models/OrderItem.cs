using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TallerIDWMBackend.Models
{
    /// <summary>
    /// Representa un artículo dentro de un pedido.
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// Identificador único del artículo en el pedido.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Identificador del pedido al que pertenece este artículo.
        /// Se relaciona con el modelo <see cref="Order"/> mediante el OrderId.
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// Identificador del producto que ha sido comprado.
        /// Se relaciona con el modelo <see cref="Product"/> mediante el ProductId.
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Información del producto comprado.
        /// Relacionado con el modelo <see cref="Product"/>.
        /// </summary>
        public Product Product { get; set; } = null!;

        /// <summary>
        /// Cantidad del producto comprada en este artículo del pedido.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Precio unitario del producto en el momento de la compra.
        /// Se utiliza para calcular el total de este artículo en el pedido.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Pedido al que pertenece este artículo.
        /// Relacionado con el modelo <see cref="Order"/>.
        /// </summary>
        public Order Order { get; set; } = null!;
    }
}
