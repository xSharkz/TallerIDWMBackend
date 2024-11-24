using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.User
{
    /// <summary>
    /// DTO utilizado para actualizar el estado de un usuario (habilitado o deshabilitado).
    /// </summary>
    public class UpdateUserStatusDto
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y corresponde al ID único del usuario cuyo estado se desea actualizar.
        /// </remarks>
        public long UserId { get; set; }

        /// <summary>
        /// Estado del usuario, indicando si está habilitado o deshabilitado.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y se utiliza para habilitar o deshabilitar la cuenta del usuario.
        /// </remarks>
        public bool IsEnabled { get; set; }
    }
}
