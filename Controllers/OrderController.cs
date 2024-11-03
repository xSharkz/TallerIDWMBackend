using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.DTOs.Order;

namespace TallerIDWMBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Endpoint para que el administrador obtenga todas las órdenes con opciones de paginación, búsqueda y ordenamiento
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrders(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? sortOrder = null)
        {
            var result = await _orderService.GetOrdersAsync(pageNumber, pageSize, searchTerm, sortOrder);
            return Ok(result);
        }

        // Endpoint para que un usuario obtenga sus propias órdenes con paginación
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserOrders(long userId, int pageNumber = 1, int pageSize = 10)
        {
            // Verificar que el usuario autenticado solo pueda acceder a sus propias órdenes
            if (User.IsInRole("User") && !User.IsInRole("Admin") && User.FindFirst("sub")?.Value != userId.ToString())
            {
                return Forbid("No tienes permiso para acceder a las órdenes de otro usuario.");
            }

            var result = await _orderService.GetOrdersByUserIdAsync(userId, pageNumber, pageSize);
            return Ok(result);
        }
    }
}
