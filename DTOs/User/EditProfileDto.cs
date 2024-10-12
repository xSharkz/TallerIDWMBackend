using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TallerIDWMBackend.Validation;

namespace TallerIDWMBackend.DTOs.User
{
    public class EditProfileDto
    {
    [Required]
    [StringLength(255, MinimumLength = 8)]
    [RegularExpression(@"^[a-zA-ZñÑáéíóúÁÉÍÓÚ ]+$", ErrorMessage = "Solo se permiten caracteres del abecedario español.")]
    public string Name { get; set; } = null!;

    [Required]
    [DataType(DataType.Date)]
    [DateInThePast(ErrorMessage = "La fecha de nacimiento debe ser anterior a la fecha actual.")]
    public DateTime BirthDate { get; set; }

    [Required]
    public string Gender { get; set; } = null!; // Femenino, Masculino, Prefiero no decirlo, Otro
    }
}