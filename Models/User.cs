using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using TallerIDWMBackend.Validations;

namespace TallerIDWMBackend.Models
{
    /// <summary>
    /// Representa un usuario del sistema, que puede ser un cliente o un administrador.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// RUT del usuario, debe ser único y válido.
        /// </summary>
        public string Rut { get; set; } = null!;

        /// <summary>
        /// Nombre del usuario. Debe tener entre 8 y 255 caracteres.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Fecha de nacimiento del usuario. Debe ser una fecha válida.
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Correo electrónico del usuario. Debe ser único y estar en formato válido.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Género del usuario. Puede ser: Femenino, Masculino, Prefiero no decirlo, Otro.
        /// </summary>
        public string Gender { get; set; } = null!;

        /// <summary>
        /// Contraseña del usuario, almacenada como un hash encriptado.
        /// </summary>
        public string PasswordHash { get; set; } = null!;

        /// <summary>
        /// Indica si el usuario es administrador. El valor predeterminado es falso.
        /// </summary>
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// Indica si la cuenta del usuario está habilitada. El valor predeterminado es verdadero.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Relación con los pedidos del usuario. Un usuario puede tener múltiples pedidos.
        /// </summary>
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
