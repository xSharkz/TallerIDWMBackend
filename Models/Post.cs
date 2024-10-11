using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.Models
{
    public class Post
    {
        public long Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Url { get; set; }
        public required string PublicId { get; set; }
    }
}