using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIDWMBackend.DTOs.Order
{
    /// <summary>
    /// Representa la dirección de entrega de un pedido.
    /// </summary>
    public class DeliveryAddressDto
    {
        /// <summary>
        /// País donde se realizará la entrega.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Ciudad donde se realizará la entrega.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Comuna donde se realizará la entrega.
        /// </summary>
        public string Commune { get; set; } = string.Empty;

        /// <summary>
        /// Calle donde se realizará la entrega.
        /// </summary>
        public string Street { get; set; } = string.Empty;
    }
}
