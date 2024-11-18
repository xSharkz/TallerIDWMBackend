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
using TallerIDWMBackend.Interfaces;

namespace TallerIDWMBackend.Controllers
{
    [ApiController] // Indica que esta clase es un controlador de API que maneja solicitudes HTTP.
    [Route("api/[controller]")] // Establece la ruta base para las solicitudes de este controlador. "[controller]" se reemplaza automáticamente con el nombre de la clase, en este caso, "cart".
    public class CartController : ControllerBase
    {
        // Declara los servicios y repositorios necesarios para la lógica del controlador.
        private readonly ApplicationDbContext _dataContext; // Contexto de la base de datos, se utiliza para interactuar con la base de datos.
        private readonly InvoiceService _invoiceService; // Servicio encargado de la lógica relacionada con las facturas.
        private readonly IProductRepository _productRepository; // Repositorio de productos para acceder a los datos de productos en la base de datos.

        // Constructor del controlador, que recibe las dependencias necesarias a través de inyección de dependencias.
        public CartController(ApplicationDbContext dataContext, InvoiceService invoiceService, IProductRepository productRepository)
        {
            // Asigna las dependencias a las variables privadas correspondientes.
            _dataContext = dataContext;
            _invoiceService = invoiceService;
            _productRepository = productRepository;
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
        public async Task<IActionResult> UpdateQuantity(long productId, [FromForm] int newQuantity)
        {
            var cartCookie = Request.Cookies["cart"];

            if (string.IsNullOrWhiteSpace(cartCookie))
            {
                return BadRequest(new { message = "El carrito está vacío." });
            }

            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartCookie);
            
            // Recuperar el producto desde la base de datos utilizando el productId
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound(new { message = "Producto no encontrado." });
            }

            // Buscar el item en el carrito y cargar el producto correspondiente
            var item = cartItems.FirstOrDefault(p => p.ProductId == productId);
            if (item == null)
            {
                return NotFound(new { message = "Producto no encontrado en el carrito." });
            }

            // Asignar el producto completo al CartItem
            item.Product = product;

            // Verificar que la cantidad sea válida
            if (newQuantity <= 0)
            {
                return BadRequest(new { message = "La cantidad debe ser mayor que 0." });
            }

            // Verificar que no se exceda el stock disponible
            if (newQuantity > product.StockQuantity)
            {
                return BadRequest(new { message = "No hay suficiente stock para esta cantidad." });
            }

            // Actualizar la cantidad en el carrito
            item.Quantity = newQuantity;

            // Guardar el carrito actualizado en la cookie
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


        [HttpPost("remove-item/{productId}")] // Define la ruta para eliminar un producto del carrito, donde {productId} es un parámetro de la URL.
        [Authorize] // Indica que este método requiere que el usuario esté autenticado.
        public IActionResult RemoveItem(long productId)
        {
            // Recupera el valor de la cookie "cart", que contiene el carrito de compras del usuario.
            var cartCookie = Request.Cookies["cart"];
            
            // Si la cookie no existe o está vacía, se retorna un error indicando que el carrito está vacío.
            if (string.IsNullOrWhiteSpace(cartCookie))
            {
                return BadRequest(new { message = "El carrito está vacío." });
            }

            // Deserializa la cookie del carrito a una lista de objetos CartItem.
            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartCookie);

            // Busca el artículo en el carrito que coincida con el ID del producto recibido.
            var item = cartItems.FirstOrDefault(p => p.ProductId == productId);
            
            // Si el producto no está en el carrito, se retorna un error indicando que no se encontró.
            if (item == null)
            {
                return NotFound(new { message = "Producto no encontrado en el carrito." });
            }

            // Elimina el producto del carrito.
            cartItems.Remove(item);

            // Vuelve a guardar el carrito actualizado en la cookie, serializando la lista de artículos.
            Response.Cookies.Append("cart", JsonConvert.SerializeObject(cartItems), new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7), // Establece la fecha de expiración de la cookie.
                HttpOnly = true, // Hace que la cookie sea accesible solo a través de HTTP, no mediante JavaScript.
                Secure = true, // Hace que la cookie se transmita solo a través de conexiones seguras (HTTPS).
                SameSite = SameSiteMode.Strict // Restringe el envío de la cookie solo a solicitudes del mismo sitio.
            });

            // Devuelve una respuesta exitosa con un mensaje y el carrito actualizado.
            return Ok(new { message = "Producto eliminado del carrito.", cartItems });
        }



        // Ruta para realizar el checkout del carrito de compras, requiere que el usuario esté autenticado con el rol de "Customer".
        [HttpPost("checkout")]
        [Authorize(Roles = "Customer")] // Requiere que el usuario tenga el rol de "Customer".
        public async Task<IActionResult> Checkout([FromForm] DeliveryAddressDto address)
        {
            // Verificar si el carrito existe y contiene productos.
            var cartCookie = Request.Cookies["cart"];
            if (string.IsNullOrWhiteSpace(cartCookie)) // Si la cookie del carrito está vacía o no existe.
            {
                return BadRequest(new { message = "El carrito está vacío." }); // Retorna un error.
            }

            // Deserializa la cookie del carrito para obtener los artículos en el carrito.
            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartCookie);
            if (cartItems == null || !cartItems.Any()) // Si no hay artículos en el carrito.
            {
                return BadRequest(new { message = "El carrito está vacío." }); // Retorna un error.
            }

            // Obtener los IDs de los productos en el carrito.
            var productIds = cartItems.Select(ci => ci.ProductId).ToList();
            
            // Obtiene los productos de la base de datos según los IDs en el carrito.
            var products = await _dataContext.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            // Procesar stock y calcular el total.
            foreach (var cartItem in cartItems)
            {
                // Asocia el producto al artículo del carrito.
                cartItem.Product = products.FirstOrDefault(p => p.Id == cartItem.ProductId);
                if (cartItem.Product == null) // Si el producto no se encuentra en la base de datos.
                {
                    return BadRequest(new { message = $"El producto con ID {cartItem.ProductId} no fue encontrado." });
                }

                // Verifica que haya suficiente stock para la cantidad del artículo en el carrito.
                if (cartItem.Product.StockQuantity < cartItem.Quantity)
                {
                    return BadRequest(new { message = $"No hay suficiente stock para el producto {cartItem.Product.Name}." });
                }

                // Actualiza el stock del producto restando la cantidad del carrito.
                cartItem.Product.StockQuantity -= cartItem.Quantity;
            }

            // Calcula el monto total de la orden sumando el precio de cada artículo por su cantidad.
            var totalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity);

            // Obtiene el ID del usuario actual desde el token JWT.
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out long userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado o token inválido." });
            }

            // Obtiene el usuario desde la base de datos usando el ID.
            var user = await _dataContext.Users.FindAsync(userId);
            if (user == null || !User.IsInRole("Customer")) // Verifica que el usuario sea un cliente.
            {
                return Unauthorized(new { message = "No autorizado. El usuario debe ser un cliente." });
            }
            
            // Obtiene la fecha y hora actual en formato UTC y luego la convierte a la hora local de Chile.
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo chileTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Santiago");
            DateTime orderDate = TimeZoneInfo.ConvertTimeFromUtc(utcNow, chileTimeZone);

            // Crea una nueva orden con los detalles del usuario, fecha, monto total y dirección de entrega.
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

            // Agrega la nueva orden a la base de datos.
            _dataContext.Orders.Add(order);
            await _dataContext.SaveChangesAsync(); // Guarda los cambios en la base de datos.

            // Genera la factura en formato PDF usando el servicio de facturación.
            var pdfBytes = _invoiceService.GenerateInvoicePdf(order, cartItems);

            // Limpia el carrito del usuario (eliminando la cookie).
            ClearCart(); // Este método es el que limpia la cookie del carrito.

            // Envía el archivo PDF generado como una respuesta descargable al cliente.
            return File(pdfBytes, "application/pdf", $"Factura_Orden_{order.Id}_IDWM.pdf");
        }



        [HttpPost("clear-cart")]
        public IActionResult ClearCart()
        {
            // Eliminar la cookie del carrito. Se establece una nueva cookie con el mismo nombre pero con una fecha de expiración en el pasado
            // Esto hace que la cookie se elimine automáticamente.
            Response.Cookies.Append("cart", "", new CookieOptions
            {
                // Establecer la cookie con una fecha de expiración en el pasado para eliminarla
                Expires = DateTimeOffset.Now.AddDays(-1),  // Se establece la fecha en el pasado para asegurar que la cookie sea eliminada
                HttpOnly = true,  // Hace que la cookie sea accesible solo desde el servidor (no desde JavaScript en el cliente)
                Secure = true,  // Solo se enviará la cookie a través de conexiones HTTPS (requiere que el servidor tenga habilitado HTTPS)
                SameSite = SameSiteMode.Strict  // Establece una política de seguridad para las cookies, limitando su envío entre sitios
            });

            // Retorna un mensaje de éxito indicando que el carrito ha sido reiniciado (eliminado)
            return Ok(new { message = "Carrito reiniciado." });
        }
    }
}