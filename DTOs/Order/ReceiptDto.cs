using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TallerIDWMBackend.DTOs.Product;

namespace TallerIDWMBackend.DTOs.Order
{
    public class ReceiptDto
    {
        public DateTime PurchaseDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<ProductDetailDto> Products { get; set; }
    }
}