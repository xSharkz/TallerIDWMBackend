using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs
{
    /// <summary>
    /// Representa los datos necesarios para que un usuario inicie sesión.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Dirección de correo electrónico del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe tener un formato de correo electrónico válido.
        /// </remarks>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe tener entre 8 y 20 caracteres.
        /// </remarks>
        [Required]
        [StringLength(20, MinimumLength = 8)]
        public string Password { get; set; } = null!;
    }
}
