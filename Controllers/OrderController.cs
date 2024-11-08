using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.DTOs.Order;
using TallerIDWMBackend.Repository;

namespace TallerIDWMBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserRepository _userRepository;

        public OrderController(IOrderService orderService, IUserRepository userRepository)
        {
            _orderService = orderService;
            _userRepository = userRepository;
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
        [HttpGet("orders")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetUserOrders(int pageNumber = 1, int pageSize = 10)
        {

            // Obtener el usuario actual usando el método GetCurrentUserAsync del repositorio
            var user = await _userRepository.GetCurrentUserAsync();

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            // Verificar que el usuario autenticado solo pueda acceder a sus propias órdenes, excepto si es administrador
            if (User.IsInRole("User") && !User.IsInRole("Admin"))
            {
                return Forbid("No tienes permiso para acceder a las órdenes de otro usuario.");
            }

            var result = await _orderService.GetOrdersByUserIdAsync(user.Id, pageNumber, pageSize);
            return Ok(result);
        }
    }
}
