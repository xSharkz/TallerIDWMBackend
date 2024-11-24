using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.Models
{
    /// <summary>
    /// Representa un producto disponible en la tienda.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Tipo o categoría del producto.
        /// Ejemplo: Poleras, Gorros, Juguetería, etc.
        /// </summary>
        public string Type { get; set; } = null!;

        /// <summary>
        /// Precio del producto en unidades de la moneda.
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// Cantidad disponible del producto en el inventario.
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// URL de la imagen del producto.
        /// Debe ser una URL válida que apunte a la imagen en línea del producto.
        /// </summary>
        public string ImageUrl { get; set; } = null!;

        /// <summary>
        /// Identificador único del producto en Cloudinary.
        /// Utilizado para gestionar la imagen en la nube.
        /// </summary>
        public string PublicId { get; set; } = null!;

        /// <summary>
        /// Relación con los artículos en el carrito de compras.
        /// Cada producto puede estar presente en varios carritos.
        /// </summary>
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        /// <summary>
        /// Relación con los artículos en un pedido.
        /// Cada producto puede estar presente en varios pedidos.
        /// </summary>
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
