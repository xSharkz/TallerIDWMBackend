using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.User
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Rut { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public bool IsEnabled { get; set; }
    }
}