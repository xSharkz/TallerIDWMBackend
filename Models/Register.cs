using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using TallerIDWMBackend.Validation;
namespace TallerIDWMBackend.Models
{
    public class Register
    {
        [Required]
        [StringLength(12)] // Asumiendo formato RUT
        public string Rut { get; set; } = null!;

        [Required]
        [StringLength(255, MinimumLength = 8)]
        [RegularExpression(@"^[a-zA-ZñÑáéíóúÁÉÍÓÚ ]+$", ErrorMessage = "Solo se permiten caracteres del abecedario español.")]
        public string Name { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        [DateInThePast(ErrorMessage = "La fecha de nacimiento debe ser anterior a la fecha actual.")]
        public DateTime BirthDate { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Gender { get; set; } = null!; // Femenino, Masculino, Prefiero no decirlo, Otro

        [Required]
        [StringLength(20, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [Required]
        [Compare("Password", ErrorMessage = "La confirmación de contraseña no coincide.")]
        public string ConfirmPassword { get; set; } = null!;
    }


}