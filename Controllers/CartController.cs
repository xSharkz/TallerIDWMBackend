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

        // 1. Visualizar carrito
        [HttpGet("view")]
        public async Task<IActionResult> ViewCart(string sessionId)
        {
            // Obtener los productos en el carrito del usuario basado en la sesión
            var cartItems = await _dataContext.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.SessionId == sessionId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return Ok(new { message = "El carrito está vacío." });
            }

            // Calcular el total
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

        // 2. Aumentar la cantidad de un producto en el carrito
        [HttpPost("increase")]
        public async Task<IActionResult> IncreaseQuantity(long cartItemId)
        {
            var cartItem = await _dataContext.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
            {
                return NotFound(new { message = "El producto no existe en el carrito." });
            }

            cartItem.Quantity++;
            await _dataContext.SaveChangesAsync();

            return Ok(new { message = "Cantidad aumentada." });
        }

        // 3. Disminuir la cantidad de un producto en el carrito
        [HttpPost("decrease")]
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
            }
            else
            {
                return BadRequest(new { message = "La cantidad mínima es 1." });
            }

            await _dataContext.SaveChangesAsync();

            return Ok(new { message = "Cantidad disminuida." });
        }

        // 4. Eliminar un producto del carrito
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveProduct(long cartItemId)
        {
            var cartItem = await _dataContext.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
            {
                return NotFound(new { message = "El producto no existe en el carrito." });
            }

            _dataContext.CartItems.Remove(cartItem);
            await _dataContext.SaveChangesAsync();

            return Ok(new { message = "Producto eliminado del carrito." });
        }

        // 5. Mostrar botón de pago (requiere inicio de sesión)
        [HttpGet("checkout")]
        public IActionResult Checkout()
        {
            // Verificar si el usuario está autenticado
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "Debe iniciar sesión para realizar el pago." });
            }

            // Si el usuario está autenticado, continuar con el proceso de pago
            return Ok(new { message = "Proceso de pago iniciado." });
        }
    }
}