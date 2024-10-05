using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TallerIDWMBackend.Interfaces;

namespace TallerIDWMBackend.Controllers
{
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository  _productRepository;
        
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
    }
}