using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TallerIDWMBackend.DTOs.Product;

namespace TallerIDWMBackend.DTOs.Order
{
    /// <summary>
    /// Representa un recibo generado después de realizar un pedido.
    /// </summary>
    public class ReceiptDto
    {
        /// <summary>
        /// Fecha en la que se realizó la compra.
        /// </summary>
        public DateTime PurchaseDate { get; set; }

        /// <summary>
        /// Precio total del pedido.
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Lista de detalles de los productos incluidos en el pedido.
        /// </summary>
        /// <remarks>
        /// Cada elemento de la lista representa información detallada de un producto en el pedido.
        /// </remarks>
        public List<ProductDetailDto> Products { get; set; }
    }
}
