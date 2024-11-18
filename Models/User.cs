using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using TallerIDWMBackend.Validations;

namespace TallerIDWMBackend.Models
{
    public class User
    {
        public long Id { get; set; }

        public string Rut { get; set; } = null!; // RUT válido y único.

        public string Name { get; set; } = null!; // Nombre entre 8 y 255 caracteres.

        public DateTime BirthDate { get; set; } // Fecha de nacimiento válida.

        public string Email { get; set; } = null!; // Correo válido y único.

        public string Gender { get; set; } = null!; // Género: Femenino, Masculino, Prefiero no decirlo, Otro.

        public string PasswordHash { get; set; } = null!; // Contraseña encriptada.

        public bool IsAdmin { get; set; } = false; // Indica si es administrador.

        public bool IsEnabled { get; set; } = true; // Indica si está habilitado.

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}