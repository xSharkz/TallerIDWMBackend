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
    [StringLength(255, MinimumLength = 8)]
    [RegularExpression(@"^[a-zA-ZñÑáéíóúÁÉÍÓÚ ]+$", ErrorMessage = "Solo se permiten caracteres del abecedario español.")]
    public string? Name { get; set; } = null!;

    [DataType(DataType.Date)]
    [DateInThePast(ErrorMessage = "La fecha de nacimiento debe ser anterior a la fecha actual.")]
    public DateTime? BirthDate { get; set; }

    [RegularExpression(@"^(Femenino|Masculino|Prefiero no decirlo|Otro)$", ErrorMessage = "El genero debe ser uno de los siguientes: Femenino, Masculino, Prefiero no decirlo, Otro.")]
    public string? Gender { get; set; } = null!; // Femenino, Masculino, Prefiero no decirlo, Otro
    }
}