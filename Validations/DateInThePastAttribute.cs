using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace TallerIDWMBackend.Validation
{
    public class DateInThePastAttribute : ValidationAttribute
    {
        public DateInThePastAttribute()
        {
            ErrorMessage = "La fecha debe ser anterior a la fecha actual.";
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success!;
            }

            if (value is DateTime dateValue)
            {
                if (dateValue < DateTime.Now)
                {
                    return ValidationResult.Success!;
                }
                else
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return new ValidationResult("El valor proporcionado no es una fecha válida.");
        }
    }
}