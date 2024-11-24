using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.Product
{
    /// <summary>
    /// DTO que representa los datos de lectura de un producto, utilizados para la visualización o consulta.
    /// </summary>
    public class ProductReadDto
    {
        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe ser una cadena no vacía.
        /// </remarks>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Tipo de producto.
        /// </summary>
        /// <remarks>
        /// Este campo debe representar una categoría o tipo específico del producto (por ejemplo, Poleras, Gorros, etc.).
        /// </remarks>
        public string Type { get; set; } = null!;

        /// <summary>
        /// Precio unitario del producto.
        /// </summary>
        /// <remarks>
        /// Este campo representa el costo por unidad del producto.
        /// </remarks>
        public int Price { get; set; }

        /// <summary>
        /// Cantidad disponible en stock del producto.
        /// </summary>
        /// <remarks>
        /// Este campo indica la cantidad disponible del producto en el inventario.
        /// </remarks>
        public int StockQuantity { get; set; }

        /// <summary>
        /// URL de la imagen del producto.
        /// </summary>
        /// <remarks>
        /// Este campo debe contener la URL de la imagen del producto, que será usada para mostrar la imagen en el frontend.
        /// </remarks>
        public string ImageUrl { get; set; } = null!;
    }
}
