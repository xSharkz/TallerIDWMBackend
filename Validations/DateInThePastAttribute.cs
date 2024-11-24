using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace TallerIDWMBackend.Validation
{
    /// <summary>
    /// Atributo de validación personalizado que verifica si una fecha es anterior a la fecha actual.
    /// </summary>
    public class DateInThePastAttribute : ValidationAttribute
    {
        /// <summary>
        /// Inicializa una nueva instancia del atributo <see cref="DateInThePastAttribute"/> y establece el mensaje de error predeterminado.
        /// </summary>
        public DateInThePastAttribute()
        {
            ErrorMessage = "La fecha debe ser anterior a la fecha actual.";
        }

        /// <summary>
        /// Valida si el valor proporcionado es una fecha que es anterior a la fecha actual.
        /// </summary>
        /// <param name="value">El valor a validar. Debe ser un objeto de tipo <see cref="DateTime"/>.</param>
        /// <param name="validationContext">El contexto de validación que contiene información sobre el objeto que se está validando.</param>
        /// <returns>Devuelve un <see cref="ValidationResult"/> que indica si la validación fue exitosa o no.</returns>
        /// <remarks>
        /// Si el valor es nulo, la validación se considera exitosa.
        /// Si el valor es un objeto <see cref="DateTime"/>, se verifica si la fecha es anterior a la fecha actual.
        /// En caso contrario, se devuelve un mensaje de error que indica que el valor no es una fecha válida.
        /// </remarks>
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