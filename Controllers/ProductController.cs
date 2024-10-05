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
        private readonly IProductRepository  _productRepository;
        
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
            var products = await _productRepository.GetPagedProductsAsync(searchQuery, type, sortOrder, pageNumber, pageSize);
            var totalItems = await _productRepository.GetTotalProductsAsync(searchQuery, type);

            return Ok(new
            {
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Products = products
            });
        }

        // POST: api/product/add/{id}
        [HttpPost("add/{id}")]
        public async Task<IActionResult> AddProduct(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);

            if (product == null || product.StockQuantity <= 0)
            {
                return NotFound("Producto no encontrado o sin stock.");
            }

            // Gestionar el carrito de compras con cookies
            var cart = Request.Cookies["cart"];
            var cartItems = string.IsNullOrEmpty(cart) ? new List<int>() : cart.Split(",").Select(int.Parse).ToList();
            cartItems.Add(id);

            // Guardar el carrito en una cookie
            Response.Cookies.Append("cart", string.Join(",", cartItems), new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7)
            });

            return Ok("Producto añadido al carrito.");
        }
        
    }
}