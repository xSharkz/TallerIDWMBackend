using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TallerIDWMBackend.DTOs.Product;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.Models;

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
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableProducts(
            string searchQuery = "", 
            string type = "", 
            string sortOrder = "asc", 
            int pageNumber = 1, 
            int pageSize = 10)
        {
            var products = await _productRepository.GetPagedProductsAsync(searchQuery, type, sortOrder, pageNumber, pageSize, includeOutOfStock: false);
            var totalItems = await _productRepository.GetTotalProductsAsync(searchQuery, type, includeOutOfStock: false);

            if (products == null || !products.Any())
            {
                return NotFound(new { message = "No se encontraron productos." });
            }

            var productDtos = products.Select(p => new ProductReadDto
            {
                Id = p.Id,
                Name = p.Name,
                Type = p.Type,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl
            });

            return Ok(new
            {
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Products = productDtos
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

            var productDto = new ProductReadDto
            {
                Id = product.Id,
                Name = product.Name,
                Type = product.Type,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl
            };

            return Ok(productDto);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllProductsForAdmin(
            string searchQuery = "", 
            string type = "", 
            string sortOrder = "asc", 
            int pageNumber = 1, 
            int pageSize = 10)
        {
            var products = await _productRepository.GetPagedProductsAsync(searchQuery, type, sortOrder, pageNumber, pageSize, includeOutOfStock: true);
            var totalItems = await _productRepository.GetTotalProductsAsync(searchQuery, type, includeOutOfStock: true);

            if (products == null || !products.Any())
            {
                return NotFound(new { message = "No se encontraron productos." });
            }

            var productDtos = products.Select(p => new ProductReadDto
            {
                Id = p.Id,
                Name = p.Name,
                Type = p.Type,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl
            });

            return Ok(new
            {
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Products = productDtos
            });
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct([FromBody] ProductCreateUpdateDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = new Product
            {
                Name = productDto.Name,
                Type = productDto.Type,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                ImageUrl = productDto.ImageUrl,
                PublicId = productDto.PublicId
            };

            try
            {
                var newProduct = await _productRepository.AddProductAsync(product);
                var newProductDto = new ProductReadDto
                {
                    Id = newProduct.Id,
                    Name = newProduct.Name,
                    Type = newProduct.Type,
                    Price = newProduct.Price,
                    StockQuantity = newProduct.StockQuantity,
                    ImageUrl = newProduct.ImageUrl
                };
                return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProductDto);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(long id, [FromBody] ProductCreateUpdateDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedProduct = new Product
            {
                Name = productDto.Name,
                Type = productDto.Type,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                ImageUrl = productDto.ImageUrl,
                PublicId = productDto.PublicId
            };

            try
            {
                var product = await _productRepository.UpdateProductAsync(id, updatedProduct);
                var updatedProductDto = new ProductReadDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Type = product.Type,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    ImageUrl = product.ImageUrl
                };
                return Ok(updatedProductDto);
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            try
            {
                await _productRepository.DeleteProductAsync(id);
                return NoContent();
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


    }
}