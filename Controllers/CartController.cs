using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerIDWMBackend.Models;
using TallerIDWMBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace TallerIDWMBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _dataContext;

        public CartController(ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        // 1. Agregar un producto al carrito
        [HttpPost("add-to-cart/{productId}")]
        [Authorize]
        public async Task<IActionResult> AddToCart(long productId)
        {
            var product = await _dataContext.Products.FindAsync(productId);

            if (product == null || product.StockQuantity <= 0)
            {
                return NotFound("Producto no encontrado o sin stock.");
            }

            // Obtener el carrito actual de las cookies (si existe)
            var cartCookie = Request.Cookies["cart"];
            List<CartItem> cartItems;

            // Verificar si la cookie está vacía o malformada
            if (string.IsNullOrEmpty(cartCookie))
            {
                cartItems = new List<CartItem>();
            }
            else
            {
                try
                {
                    // Intentar deserializar la cookie como una lista de CartItem
                    cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartCookie);

                    // Asegurarse de que sea una lista válida
                    if (cartItems == null)
                    {
                        return BadRequest("El carrito no contiene datos válidos.");
                    }
                }
                catch (JsonSerializationException)
                {
                    // Manejar el caso en que la cookie esté corrupta o malformada
                    return BadRequest("Error al deserializar el carrito. Reinicie el carrito.");
                }
            }

            // Verificar si el producto ya está en el carrito
            var existingCartItem = cartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (existingCartItem != null)
            {
                // Incrementar la cantidad si ya existe en el carrito
                existingCartItem.Quantity++;
            }
            else
            {
                // Añadir nuevo producto al carrito (solo con ProductId y Quantity)
                cartItems.Add(new CartItem
                {
                    ProductId = product.Id,
                    Quantity = 1,
                    SessionId = "your-session-id" // Cambia esto según cómo gestiones la sesión
                });
            }

            // Guardar el carrito actualizado en las cookies
            Response.Cookies.Append("cart", JsonConvert.SerializeObject(cartItems), new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { message = "Producto añadido al carrito." });
        }


        [HttpPost("update-quantity/{productId}")]
        [Authorize]
        public IActionResult UpdateQuantity(long productId, [FromBody] int newQuantity)
        {
            var cartCookie = Request.Cookies["cart"];
            
            if (string.IsNullOrWhiteSpace(cartCookie))
            {
                return BadRequest(new { message = "El carrito está vacío." });
            }

            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartCookie);
            var item = cartItems.FirstOrDefault(p => p.ProductId == productId);
            if (item == null)
            {
                return NotFound(new { message = "Producto no encontrado en el carrito." });
            }

            if (newQuantity <= 0)
            {
                return BadRequest(new { message = "La cantidad debe ser mayor que 0." });
            }

            item.Quantity = newQuantity;

            // Guardar el carrito actualizado
            Response.Cookies.Append("cart", JsonConvert.SerializeObject(cartItems), new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { message = "Cantidad actualizada.", cartItems });
        }


        // 4. Visualizar carrito
        [HttpGet("view")]
        [Authorize]
        public async Task<IActionResult> ViewCart()
        {
            // Obtener la cookie del carrito
            var cartCookie = Request.Cookies["cart"];

            if (string.IsNullOrWhiteSpace(cartCookie))
            {
                return Ok(new { message = "El carrito está vacío." });
            }

            // Deserializar el carrito desde las cookies
            List<CartItem> cartItems;
            try
            {
                cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartCookie);
            }
            catch (JsonSerializationException)
            {
                return BadRequest("Error al deserializar el carrito.");
            }

            if (cartItems == null || !cartItems.Any())
            {
                return Ok(new { message = "El carrito está vacío." });
            }

            // Obtener la lista de productos desde la base de datos
            var productIds = cartItems.Select(ci => ci.ProductId).ToList();
            var products = await _dataContext.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            // Asociar los productos con los CartItems
            foreach (var cartItem in cartItems)
            {
                cartItem.Product = products.FirstOrDefault(p => p.Id == cartItem.ProductId);
            }

            // Calcular el total a pagar
            var totalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);

            return Ok(new
            {
                Items = cartItems.Select(ci => new
                {
                    ci.Product.Name,
                    ci.Product.Price,
                    ci.Quantity,
                    Subtotal = ci.Product.Price * ci.Quantity
                }),
                Total = totalAmount
            });
        }


        [HttpPost("remove-item/{productId}")]
        [Authorize]
        public IActionResult RemoveItem(long productId)
        {
            var cartCookie = Request.Cookies["cart"];
            
            if (string.IsNullOrWhiteSpace(cartCookie))
            {
                return BadRequest(new { message = "El carrito está vacío." });
            }

            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartCookie);
            var item = cartItems.FirstOrDefault(p => p.ProductId == productId);
            if (item == null)
            {
                return NotFound(new { message = "Producto no encontrado en el carrito." });
            }

            cartItems.Remove(item);

            Response.Cookies.Append("cart", JsonConvert.SerializeObject(cartItems), new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { message = "Producto eliminado del carrito.", cartItems });
        }


        // 5. Mostrar botón de pago (requiere inicio de sesión)
        [HttpGet("checkout")]
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var cartCookie = Request.Cookies["cart"];
            
            if (string.IsNullOrWhiteSpace(cartCookie))
            {
                return BadRequest(new { message = "El carrito está vacío." });
            }

            // Deserializar la cookie que contiene el carrito
            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartCookie);
            if (cartItems == null || !cartItems.Any())
            {
                return BadRequest(new { message = "El carrito está vacío." });
            }

            // Obtener los IDs de los productos en el carrito
            var productIds = cartItems.Select(ci => ci.ProductId).ToList();

            // Cargar los productos desde la base de datos
            var products = await _dataContext.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            // Asignar los productos correspondientes a los CartItems
            foreach (var cartItem in cartItems)
            {
                cartItem.Product = products.FirstOrDefault(p => p.Id == cartItem.ProductId);
                if (cartItem.Product == null)
                {
                    return BadRequest(new { message = $"El producto con ID {cartItem.ProductId} no fue encontrado." });
                }
                // Verificar si hay suficiente stock para cada producto
                var product = _dataContext.Products.First(p => p.Id == cartItem.ProductId);
                if (product.StockQuantity < cartItem.Quantity)
                {
                    return BadRequest(new { message = $"No hay suficiente stock para el producto {product.Name}." });
                }

                // Asignar el producto y descontar el stock
                cartItem.Product = product;
                product.StockQuantity -= cartItem.Quantity;
                await _dataContext.SaveChangesAsync();
            }

            // Calcular el total a pagar
            var total = cartItems.Sum(item => item.Product.Price * item.Quantity);
            ClearCart();
            return Ok(new
            {
                Items = cartItems.Select(item => new
                {
                    ProductName = item.Product.Name,
                    ProductPrice = item.Product.Price,
                    item.Quantity,
                    Subtotal = item.Product.Price * item.Quantity
                }),
                Total = total
            });
        }

        [HttpPost("clear-cart")]
        public IActionResult ClearCart()
        {
            // Eliminar la cookie del carrito
            Response.Cookies.Append("cart", "", new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(-1), // Establecer la cookie con una fecha en el pasado para eliminarla
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { message = "Carrito reiniciado." });
        }

    }
}