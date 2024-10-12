using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TallerIDWMBackend.Interfaces;

namespace TallerIDWMBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetProducts(
            string searchQuery = "", 
            string type = "", 
            string sortOrder = "asc", 
            int pageNumber = 1, 
            int pageSize = 10)
        {
            // Limitar los valores de pageSize y pageNumber
            if (pageSize < 1 || pageSize > 100)
            {
                pageSize = 10; // Límite predeterminado
            }

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            var products = await _productRepository.GetPagedProductsAsync(searchQuery, type, sortOrder, pageNumber, pageSize);
            var totalItems = await _productRepository.GetTotalProductsAsync(searchQuery, type);

            if (products == null || !products.Any())
            {
                return NotFound(new { message = "No se encontraron productos." });
            }

            return Ok(new
            {
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Products = products.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Type,
                    p.Price,
                    p.StockQuantity,
                    p.ImageUrl
                })
            });
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(long id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Producto no encontrado." });
            }

            return Ok(new
            {
                product.Id,
                product.Name,
                product.Type,
                product.Price,
                product.StockQuantity,
                product.ImageUrl
            });
        }
    }
}