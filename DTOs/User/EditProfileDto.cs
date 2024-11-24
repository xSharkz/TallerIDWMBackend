using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TallerIDWMBackend.Validation;

namespace TallerIDWMBackend.DTOs.User
{
    /// <summary>
    /// DTO utilizado para editar el perfil de un usuario.
    /// </summary>
    public class EditProfileDto
    {
        /// <summary>
        /// Nombre del usuario. 
        /// </summary>
        /// <remarks>
        /// Este campo es opcional, debe tener una longitud mínima de 8 caracteres y un máximo de 255.
        /// Solo se permiten caracteres del abecedario español.
        /// </remarks>
        [StringLength(255, MinimumLength = 8)]
        [RegularExpression(@"^[a-zA-ZñÑáéíóúÁÉÍÓÚ ]+$", ErrorMessage = "Solo se permiten caracteres del abecedario español.")]
        public string? Name { get; set; } = null!;

        /// <summary>
        /// Fecha de nacimiento del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo es opcional. La fecha debe ser anterior a la fecha actual.
        /// </remarks>
        [DataType(DataType.Date)]
        [DateInThePast(ErrorMessage = "La fecha de nacimiento debe ser anterior a la fecha actual.")]
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Género del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo es opcional y debe ser uno de los siguientes valores: 
        /// "Femenino", "Masculino", "Prefiero no decirlo", "Otro".
        /// </remarks>
        [RegularExpression(@"^(Femenino|Masculino|Prefiero no decirlo|Otro)$", ErrorMessage = "El genero debe ser uno de los siguientes: Femenino, Masculino, Prefiero no decirlo, Otro.")]
        public string? Gender { get; set; } = null!; // Femenino, Masculino, Prefiero no decirlo, Otro
    }
}
