using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TallerIDWMBackend.Models
{
    /// <summary>
    /// Representa un pedido realizado por un cliente en la tienda.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Identificador único del pedido.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Identificador del usuario (cliente) que realizó el pedido.
        /// Se relaciona con el modelo <see cref="User"/> mediante el UserId.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Fecha en que se realizó el pedido.
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Monto total que debe pagar el cliente por el pedido.
        /// Este campo almacena el total acumulado de todos los artículos en el pedido.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Dirección de entrega del pedido.
        /// Se requiere que esta dirección sea proporcionada por el cliente durante la compra.
        /// </summary>
        public string DeliveryAddress { get; set; } = null!;

        /// <summary>
        /// Colección de artículos incluidos en el pedido.
        /// Relacionado con el modelo <see cref="OrderItem"/>.
        /// </summary>
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
