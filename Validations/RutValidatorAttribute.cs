using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TallerIDWMBackend.Validations
{
    public class RutValidatorAttribute : ValidationAttribute
    {
        public RutValidatorAttribute()
        {
            ErrorMessage = "El rut ingresado no es válido.";
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            // Comprobar si el valor es nulo
            if (value == null)
            {
                return new ValidationResult("El RUT no puede ser nulo.");
            }

            string rut = value.ToString()?.Replace(".", "").Replace(" ", "").ToUpper(); // Eliminar puntos y espacios y convertir a mayúsculas

            // Comprobar que tenga el formato correcto (7 o 8 dígitos seguidos de un guion y el dígito verificador)
            if (!Regex.IsMatch(rut, @"^\d{7,8}-[\dK]$"))
            {
                return new ValidationResult(ErrorMessage);
            }

            // Separar el número y el dígito verificador
            string[] rutParts = rut.Split('-');
            string rutNumbers = rutParts[0];
            string rutCheckDigit = rutParts[1];

            // Calcular el dígito verificador
            int calculatedDigit = CalculateCheckDigit(rutNumbers);

            // Verificar si el dígito verificador es válido
            if ((rutCheckDigit == "K" && calculatedDigit == 10) || rutCheckDigit == calculatedDigit.ToString())
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage); // Si no es válido, devuelve el mensaje de error
        }

        private static int CalculateCheckDigit(string rutNumbers)
        {
            int[] multipliers = new int[] { 2, 3, 4, 5, 6, 7, 2, 3, 4 }; // Los multiplicadores para el cálculo
            int invertedRut = 0;

            // Invertir el RUT para facilitar el cálculo
            for (int i = rutNumbers.Length - 1, j = 0; i >= 0; i--, j++)
            {
                invertedRut += int.Parse(rutNumbers[i].ToString()) * multipliers[j];
            }

            int result = 11 - (invertedRut % 11);

            // Devolver el dígito verificador según el resultado
            if (result == 11)
                return 0;
            else if (result == 10)
                return 10;
            else
                return result;
        }
    }
}
