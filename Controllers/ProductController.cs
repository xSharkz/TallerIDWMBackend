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
    // Ruta base para las solicitudes de este controlador
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;  // Repositorio de productos para interactuar con la base de datos
        private readonly IPhotoService _photoService;  // Servicio de fotos para gestionar imágenes de productos

        // Constructor que inyecta las dependencias necesarias
        public ProductController(IProductRepository productRepository, IPhotoService photoService)
        {
            _productRepository = productRepository;
            _photoService = photoService;
        }

        // Endpoint para obtener productos disponibles con opciones de paginación y búsqueda
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableProducts(
            string searchQuery = "",  // Parámetro para filtrar productos por nombre
            string type = "",  // Filtro por tipo de producto
            string sortOrder = "asc",  // Ordenamiento ascendente o descendente
            int pageNumber = 1,  // Número de página para la paginación
            int pageSize = 10)  // Tamaño de página para la paginación
        {
            var products = await _productRepository.GetPagedProductsAsync(searchQuery, type, sortOrder, pageNumber, pageSize, includeOutOfStock: false);
            var totalItems = await _productRepository.GetTotalProductsAsync(searchQuery, type, includeOutOfStock: false);

            if (products == null || !products.Any())  // Si no se encuentran productos
            {
                return NotFound(new { message = "No se encontraron productos." });
            }

            // Transformar los productos en DTOs para la respuesta
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
                TotalItems = totalItems,  // Total de productos encontrados
                PageNumber = pageNumber,  // Página actual
                PageSize = pageSize,  // Tamaño de página
                Products = productDtos  // Lista de productos en formato DTO
            });
        }

        // Endpoint para obtener un producto por su ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(long id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)  // Si el producto no se encuentra
            {
                return NotFound(new { message = "Producto no encontrado." });
            }

            // Transformar el producto en DTO para la respuesta
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

        // Endpoint para obtener todos los productos con acceso restringido a administradores
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]  // Solo accesible por administradores
        public async Task<IActionResult> GetAllProductsForAdmin(
            string searchQuery = "", 
            string type = "", 
            string sortOrder = "asc", 
            int pageNumber = 1, 
            int pageSize = 10)
        {
            var products = await _productRepository.GetPagedProductsAsync(searchQuery, type, sortOrder, pageNumber, pageSize, includeOutOfStock: true);
            var totalItems = await _productRepository.GetTotalProductsAsync(searchQuery, type, includeOutOfStock: true);

            if (products == null || !products.Any())  // Si no se encuentran productos
            {
                return NotFound(new { message = "No se encontraron productos." });
            }

            // Transformar los productos en DTOs para la respuesta
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
                TotalItems = totalItems,  // Total de productos encontrados
                PageNumber = pageNumber,  // Página actual
                PageSize = pageSize,  // Tamaño de página
                Products = productDtos  // Lista de productos en formato DTO
            });
        }

        // Endpoint para agregar un nuevo producto
        [HttpPost]
        [Authorize(Roles = "Admin")]  // Solo accesible por administradores
        public async Task<IActionResult> AddProduct([FromForm] ProductCreateUpdateDto productDto)
        {
            if (!ModelState.IsValid)  // Verificar si el modelo es válido
            {
                return BadRequest(ModelState);
            }

            ImageUploadResult uploadResult = null;
            if (productDto.File != null)  // Si se proporciona una imagen
            {
                uploadResult = await _photoService.AddPhotoAsync(productDto.File);
                if (uploadResult.Error != null)  // Si hubo un error al subir la imagen
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

        // Endpoint para actualizar un producto existente
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]  // Solo accesible por administradores
        public async Task<IActionResult> UpdateProduct(long id, [FromForm] ProductCreateUpdateDto productDto)
        {
            if (!ModelState.IsValid)  // Verificar si el modelo es válido
            {
                return BadRequest(ModelState);
            }

            var productToUpdate = await _productRepository.GetProductByIdAsync(id);
            if (productToUpdate == null)  // Si el producto no existe
            {
                return NotFound(new { message = "Producto no encontrado." });
            }

            ImageUploadResult uploadResult = null;
            if (productDto.File != null)  // Si se proporciona una nueva imagen
            {
                if (!string.IsNullOrEmpty(productToUpdate.PublicId))  // Eliminar la foto anterior si existe
                {
                    await _photoService.DeletePhotoAsync(productToUpdate.PublicId);
                }

                uploadResult = await _photoService.AddPhotoAsync(productDto.File);
                if (uploadResult.Error != null)  // Si hubo un error al subir la imagen
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
        // Endpoint para eliminar un producto
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]// Solo accesible por administradores
        public async Task<IActionResult> DeleteProduct(long id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product != null && !string.IsNullOrEmpty(product.PublicId)) // Si el producto tiene una imagen asociada
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