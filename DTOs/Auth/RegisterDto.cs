using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TallerIDWMBackend.Validation;
using TallerIDWMBackend.Validations;

namespace TallerIDWMBackend.DTOs.Auth
{
    /// <summary>
    /// Representa los datos necesarios para registrar un nuevo usuario.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// RUT del usuario (Rol Único Tributario).
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio, debe tener un máximo de 12 caracteres y ser válido según la regla especificada en <see cref="RutValidatorAttribute"/>.
        /// </remarks>
        [Required]
        [StringLength(12)]
        [RutValidatorAttribute(ErrorMessage = "El rut ingresado no es válido.")]
        public string Rut { get; set; } = null!;

        /// <summary>
        /// Nombre completo del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio, debe tener entre 8 y 255 caracteres y solo permite letras del alfabeto español, incluyendo tildes y espacios.
        /// </remarks>
        [Required]
        [StringLength(255, MinimumLength = 8)]
        [RegularExpression(@"^[a-zA-ZñÑáéíóúÁÉÍÓÚ ]+$", ErrorMessage = "Solo se permiten caracteres del abecedario español.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Fecha de nacimiento del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe ser una fecha anterior a la actual, validada mediante <see cref="DateInThePast"/>.
        /// </remarks>
        [Required]
        [DataType(DataType.Date)]
        [DateInThePast(ErrorMessage = "La fecha de nacimiento debe ser anterior a la fecha actual.")]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe tener un formato válido de correo electrónico.
        /// </remarks>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Género del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe coincidir con uno de los valores permitidos: "Femenino", "Masculino", "Prefiero no decirlo", o "Otro".
        /// </remarks>
        [Required]
        [RegularExpression(@"^(Femenino|Masculino|Prefiero no decirlo|Otro)$", ErrorMessage = "El genero debe ser uno de los siguientes: Femenino, Masculino, Prefiero no decirlo, Otro.")]
        public string Gender { get; set; } = null!;

        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe tener entre 8 y 20 caracteres.
        /// </remarks>
        [Required]
        [StringLength(20, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        /// <summary>
        /// Confirmación de la contraseña del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe coincidir con el valor de <see cref="Password"/>.
        /// </remarks>
        [Required]
        [Compare("Password", ErrorMessage = "La confirmación de contraseña no coincide.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
