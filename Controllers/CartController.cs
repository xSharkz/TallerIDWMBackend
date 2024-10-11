using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerIDWMBackend.Models;
using TallerIDWMBackend.Data;

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
        public async Task<IActionResult> AddToCart(long productId, string sessionId)
        {
            var product = await _dataContext.Products.FindAsync(productId);

            if (product == null || product.StockQuantity <= 0)
            {
                return NotFound("Producto no encontrado o sin stock.");
            }

            // Verificar si el producto ya está en el carrito
            var existingCartItem = await _dataContext.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.SessionId == sessionId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity++;
            }
            else
            {
                var newCartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = 1,
                    SessionId = sessionId
                };
                await _dataContext.CartItems.AddAsync(newCartItem);
            }

            await _dataContext.SaveChangesAsync();

            return Ok("Producto añadido al carrito.");
        }

        // 2. Aumentar la cantidad de un producto en el carrito
        [HttpPost("increase/{cartItemId}")]
        public async Task<IActionResult> IncreaseQuantity(long cartItemId, string sessionId)
        {
            // Asegúrate de que el sessionId se pase como parámetro
            var cartItem = await _dataContext.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.SessionId == sessionId);

            if (cartItem == null)
            {
                return NotFound(new { message = "El producto no existe en el carrito." });
            }

            cartItem.Quantity++;
            await _dataContext.SaveChangesAsync();

            return Ok(new { message = "Cantidad aumentada." });
        }

        // 3. Disminuir la cantidad de un producto en el carrito
        [HttpPost("decrease/{cartItemId}")]
        public async Task<IActionResult> DecreaseQuantity(long cartItemId)
        {
            var cartItem = await _dataContext.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
            {
                return NotFound(new { message = "El producto no existe en el carrito." });
            }

            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                await _dataContext.SaveChangesAsync();
                return Ok(new { message = "Cantidad disminuida." });
            }
            else
            {
                return BadRequest(new { message = "La cantidad mínima es 1." });
            }
        }

        // 4. Visualizar carrito
        [HttpGet("view")]
        public async Task<IActionResult> ViewCart(string sessionId)
        {
            var cartItems = await _dataContext.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.SessionId == sessionId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return Ok(new { message = "El carrito está vacío." });
            }

            var totalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);

            return Ok(new
            {
                Items = cartItems.Select(ci => new
                {
                    ci.Id,
                    ci.Product.Name,
                    ci.Product.Price,
                    ci.Quantity,
                    Subtotal = ci.Product.Price * ci.Quantity
                }),
                Total = totalAmount
            });
        }

        // 5. Mostrar botón de pago (requiere inicio de sesión)
        [HttpGet("checkout")]
        public IActionResult Checkout()
        {
            Console.WriteLine($"Is Authenticated: {User.Identity.IsAuthenticated}");
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "Debe iniciar sesión para realizar el pago." });
            }

            return Ok(new { message = "Proceso de pago iniciado." });
        }

        // 6. Eliminar un producto del carrito
        [HttpPost("remove/{id}")]
        public async Task<IActionResult> RemoveFromCart(long id, string sessionId)
        {
            var cartItem = await _dataContext.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == id && ci.SessionId == sessionId);

            if (cartItem == null)
            {
                return BadRequest("El producto no está en el carrito.");
            }

            _dataContext.CartItems.Remove(cartItem);
            await _dataContext.SaveChangesAsync();

            return Ok("Producto eliminado del carrito.");
        }
    }
}