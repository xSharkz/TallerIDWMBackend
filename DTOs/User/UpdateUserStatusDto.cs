using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.User
{
    public class UpdateUserStatusDto
    {
        public long UserId { get; set; }
        public bool IsEnabled { get; set; }
    }
}