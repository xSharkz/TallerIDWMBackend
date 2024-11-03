using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.Product
{
    public class ProductCreateUpdateDto
    {
        [Required]
        [StringLength(64, MinimumLength = 10, ErrorMessage = "El nombre debe tener entre 10 y 64 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre solo debe contener letras y espacios.")]
        public string Name { get; set; } = null!;

        [Required]
        [RegularExpression(@"^(Poleras|Gorros|Juguetería|Alimentación|Libros)$", ErrorMessage = "El tipo debe ser uno de los siguientes: Poleras, Gorros, Juguetería, Alimentación, Libros.")]
        public string Type { get; set; } = null!;

        [Required]
        [Range(0, 99999999, ErrorMessage = "El precio debe ser un número entero positivo menor que 100 millones.")]
        public int Price { get; set; }

        [Required]
        [Range(0, 99999, ErrorMessage = "La cantidad en stock debe ser un número entero no negativo menor que 100 mil.")]
        public int StockQuantity { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        [RegularExpression(@"^(https?://.*\.(png|jpg))$", ErrorMessage = "La URL de la imagen debe ser un enlace a un archivo .png o .jpg.")]
        public string ImageUrl { get; set; } = null!;

        public string PublicId { get; set; } = null!;
    }
}