using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.Product
{
    /// <summary>
    /// DTO para la creación o actualización de un producto.
    /// </summary>
    public class ProductCreateUpdateDto
    {
        /// <summary>
        /// Nombre del producto.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe contener entre 10 y 64 caracteres. Solo se permiten letras y espacios.
        /// </remarks>
        [Required]
        [StringLength(64, MinimumLength = 10, ErrorMessage = "El nombre debe tener entre 10 y 64 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre solo debe contener letras y espacios.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Tipo de producto.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe ser uno de los siguientes valores: Poleras, Gorros, Juguetería, Alimentación, Libros.
        /// </remarks>
        [Required]
        [RegularExpression(@"^(Poleras|Gorros|Juguetería|Alimentación|Libros)$", ErrorMessage = "El tipo debe ser uno de los siguientes: Poleras, Gorros, Juguetería, Alimentación, Libros.")]
        public string Type { get; set; } = null!;

        /// <summary>
        /// Precio del producto.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe ser un número entero positivo menor que 100 millones.
        /// </remarks>
        [Required]
        [Range(0, 99999999, ErrorMessage = "El precio debe ser un número entero positivo menor que 100 millones.")]
        public int Price { get; set; }

        /// <summary>
        /// Cantidad en stock del producto.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe ser un número entero no negativo menor que 100 mil.
        /// </remarks>
        [Required]
        [Range(0, 99999, ErrorMessage = "La cantidad en stock debe ser un número entero no negativo menor que 100 mil.")]
        public int StockQuantity { get; set; }

        /// <summary>
        /// Archivo de imagen del producto.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y se espera que sea un archivo cargado por el usuario.
        /// </remarks>
        [Required]
        [DataType(DataType.Upload)]
        public IFormFile File { get; set; } = null!;
    }
}
