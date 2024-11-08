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
using TallerIDWMBackend.Services;
using TallerIDWMBackend.DTOs.Order;
using System.Security.Claims;

namespace TallerIDWMBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _dataContext;
        private readonly InvoiceService _invoiceService;

        public CartController(ApplicationDbContext dataContext, InvoiceService invoiceService)
        {
            _dataContext = dataContext;
            _invoiceService = invoiceService;
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
                    cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartCookie) ?? new List<CartItem>(); // Si la deserialización devuelve null, usar una lista vacía
                }
                catch (JsonSerializationException)
                {
                    // Manejar el caso en que la cookie esté corrupta o malformada
                    ClearCart();
                    return BadRequest("Error al deserializar el carrito. Reinicie el carrito.");
                }
            }

            // Verifica si el usuario está autenticado
            string sessionId;
            if (User.Identity.IsAuthenticated)
            {
                sessionId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Usar el UserId del usuario autenticado
            }
            else
            {
                sessionId = "guest-" + Guid.NewGuid().ToString();  // Usar un GUID para un usuario invitado
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
                    SessionId = sessionId
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
            if (newQuantity > item.Product.StockQuantity)
            {
                return BadRequest(new { message = "No hay suficiente stock para esta cantidad." });
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
        [HttpPost("checkout")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Checkout([FromBody] DeliveryAddressDto address)
        {
            // Verificar si el carrito existe y contiene productos
            var cartCookie = Request.Cookies["cart"];
            if (string.IsNullOrWhiteSpace(cartCookie))
            {
                return BadRequest(new { message = "El carrito está vacío." });
            }

            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartCookie);
            if (cartItems == null || !cartItems.Any())
            {
                return BadRequest(new { message = "El carrito está vacío." });
            }

            // Obtener los IDs de los productos en el carrito
            var productIds = cartItems.Select(ci => ci.ProductId).ToList();
            var products = await _dataContext.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            // Procesar stock y calcular total
            foreach (var cartItem in cartItems)
            {
                cartItem.Product = products.FirstOrDefault(p => p.Id == cartItem.ProductId);
                if (cartItem.Product == null)
                {
                    return BadRequest(new { message = $"El producto con ID {cartItem.ProductId} no fue encontrado." });
                }
                if (cartItem.Product.StockQuantity < cartItem.Quantity)
                {
                    return BadRequest(new { message = $"No hay suficiente stock para el producto {cartItem.Product.Name}." });
                }
                cartItem.Product.StockQuantity -= cartItem.Quantity;
            }
            
            var totalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity);
            
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out long userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado o token inválido." });
            }

            var user = await _dataContext.Users.FindAsync(userId);
            if (user == null || !User.IsInRole("Customer"))
            {
                return Unauthorized(new { message = "No autorizado. El usuario debe ser un cliente." });
            }
            
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo chileTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Santiago");
            // Convierte la hora UTC a la hora local de Chile
            DateTime orderDate = TimeZoneInfo.ConvertTimeFromUtc(utcNow, chileTimeZone);

            // Crear la orden
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = orderDate,
                TotalAmount = totalAmount,
                DeliveryAddress = $"{address.Country}, {address.City}, {address.Commune}, {address.Street}",
                OrderItems = cartItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                }).ToList()
            };

            _dataContext.Orders.Add(order);
            await _dataContext.SaveChangesAsync();

            // Generar la factura en PDF
            var pdfBytes = _invoiceService.GenerateInvoicePdf(order, cartItems);

            // Limpiar el carrito
            ClearCart();

            // Enviar el PDF como archivo descargable
            return File(pdfBytes, "application/pdf", $"Factura_Orden_{order.Id}_IDWM.pdf");
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