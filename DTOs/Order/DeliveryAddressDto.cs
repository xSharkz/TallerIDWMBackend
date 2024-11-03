using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.Order
{
    public class DeliveryAddressDto
    {
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Commune { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
    }
}