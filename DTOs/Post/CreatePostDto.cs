using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.Post
{
    public class CreatePostDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required IFormFile Image { get; set; }
    }
}