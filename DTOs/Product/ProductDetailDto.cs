using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.Product
{
    /// <summary>
    /// DTO que proporciona detalles sobre un producto en un contexto específico, como un pedido o carrito de compras.
    /// </summary>
    public class ProductDetailDto
    {
        /// <summary>
        /// Nombre del producto.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tipo de producto.
        /// </summary>
        /// <remarks>
        /// Puede ser uno de los valores predefinidos, como Poleras, Gorros, Juguetería, Alimentación o Libros.
        /// </remarks>
        public string Type { get; set; }

        /// <summary>
        /// Precio unitario del producto.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Cantidad del producto.
        /// </summary>
        /// <remarks>
        /// Representa la cantidad de unidades del producto involucradas en la transacción.
        /// </remarks>
        public int Quantity { get; set; }

        /// <summary>
        /// Precio total calculado del producto.
        /// </summary>
        /// <remarks>
        /// Este valor se calcula como el producto de <c>UnitPrice</c> y <c>Quantity</c>.
        /// </remarks>
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
