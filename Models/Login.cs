using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace TallerIDWMBackend.Models
{
    public class Login
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(20, MinimumLength = 8)]
        public string Password { get; set; } = null!;
    }
}