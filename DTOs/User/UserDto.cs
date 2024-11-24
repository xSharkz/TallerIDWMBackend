using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.User
{
    /// <summary>
    /// DTO que representa un usuario dentro del sistema.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y corresponde al ID único del usuario en la base de datos.
        /// </remarks>
        public long Id { get; set; }

        /// <summary>
        /// RUT del usuario (número de identificación nacional en Chile).
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe cumplir con el formato de un RUT válido.
        /// </remarks>
        public string Rut { get; set; } = null!;

        /// <summary>
        /// Nombre completo del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe contener entre 8 y 255 caracteres.
        /// </remarks>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe cumplir con el formato estándar de un correo electrónico.
        /// </remarks>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Género del usuario.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y debe ser uno de los siguientes valores: 
        /// "Femenino", "Masculino", "Prefiero no decirlo", o "Otro".
        /// </remarks>
        public string Gender { get; set; } = null!;

        /// <summary>
        /// Indica si la cuenta del usuario está habilitada o no.
        /// </summary>
        /// <remarks>
        /// Este campo es obligatorio y se utiliza para determinar si un usuario está habilitado para acceder al sistema.
        /// </remarks>
        public bool IsEnabled { get; set; }
    }
}
