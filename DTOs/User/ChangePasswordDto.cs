using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.User
{
    public class ChangePasswordDto
    {
        [Required]
        [StringLength(20, MinimumLength = 8)]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [StringLength(20, MinimumLength = 8)]
        public string NewPassword { get; set; } = null!;

        [Required]
        [Compare("NewPassword", ErrorMessage = "La confirmación de contraseña no coincide.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}