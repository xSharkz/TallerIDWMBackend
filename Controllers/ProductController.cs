using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
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
        private readonly IPhotoService _photoService;

        public ProductController(IProductRepository productRepository, IPhotoService photoService)
        {
            _productRepository = productRepository;
            _photoService = photoService;
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
        public async Task<IActionResult> AddProduct([FromForm] ProductCreateUpdateDto productDto) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Subir la imagen si se proporciona
            ImageUploadResult uploadResult = null;
            if (productDto.File != null)
            {
                uploadResult = await _photoService.AddPhotoAsync(productDto.File);
                if (uploadResult.Error != null)
                {
                    return BadRequest(new { message = "Error al subir la imagen", error = uploadResult.Error });
                }
            }

            var product = new Product
            {
                Name = productDto.Name,
                Type = productDto.Type,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                ImageUrl = uploadResult?.SecureUrl?.ToString(),
                PublicId = uploadResult?.PublicId,
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
        public async Task<IActionResult> UpdateProduct(long id, [FromBody] ProductCreateUpdateDto productDto, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productToUpdate = await _productRepository.GetProductByIdAsync(id);
            if (productToUpdate == null)
            {
                return NotFound(new { message = "Producto no encontrado." });
            }

            ImageUploadResult uploadResult = null;
            if (file != null)
            {
                // Eliminar la foto anterior si se proporciona una nueva
                if (!string.IsNullOrEmpty(productToUpdate.PublicId))
                {
                    await _photoService.DeletePhotoAsync(productToUpdate.PublicId);
                }

                uploadResult = await _photoService.AddPhotoAsync(file);
                if (uploadResult.Error != null)
                {
                    return BadRequest(new { message = "Error al subir la imagen", error = uploadResult.Error });
                }
            }

            var updatedProduct = new Product
            {
                Id = id,
                Name = productDto.Name,
                Type = productDto.Type,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                ImageUrl = uploadResult?.SecureUrl?.ToString() ?? productToUpdate.ImageUrl,
                PublicId = uploadResult?.PublicId ?? productToUpdate.PublicId
            };

            try
            {
                var updatedProductEntity = await _productRepository.UpdateProductAsync(id, updatedProduct);
                var updatedProductDto = new ProductReadDto
                {
                    Id = updatedProductEntity.Id,
                    Name = updatedProductEntity.Name,
                    Type = updatedProductEntity.Type,
                    Price = updatedProductEntity.Price,
                    StockQuantity = updatedProductEntity.StockQuantity,
                    ImageUrl = updatedProductEntity.ImageUrl
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
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product != null && !string.IsNullOrEmpty(product.PublicId))
                {
                    // Eliminar la imagen asociada antes de borrar el producto
                    await _photoService.DeletePhotoAsync(product.PublicId);
                }

                await _productRepository.DeleteProductAsync(id);
                return Ok( new { message = "Producto eliminado exitosamente."});
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}