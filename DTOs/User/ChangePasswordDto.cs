using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.User
{
    /// <summary>
    /// DTO utilizado para cambiar la contraseña de un usuario.
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// Contraseña actual del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe tener una longitud entre 8 y 20 caracteres.
        /// </remarks>
        [Required]
        [StringLength(20, MinimumLength = 8)]
        public string CurrentPassword { get; set; } = null!;

        /// <summary>
        /// Nueva contraseña que el usuario desea establecer.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe tener una longitud entre 8 y 20 caracteres.
        /// </remarks>
        [Required]
        [StringLength(20, MinimumLength = 8)]
        public string NewPassword { get; set; } = null!;

        /// <summary>
        /// Confirmación de la nueva contraseña.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe coincidir con el campo <c>NewPassword</c> para asegurar que la nueva contraseña es correcta.
        /// </remarks>
        [Required]
        [Compare("NewPassword", ErrorMessage = "La confirmación de contraseña no coincide.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
