using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.DTOs.Order;
using TallerIDWMBackend.Repository;
using System.Security.Claims;

namespace TallerIDWMBackend.Controllers
{
    [ApiController]  // Indica que la clase es un controlador de API
    [Route("api/[controller]")]  // Define la ruta base para este controlador
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;  // Servicio para manejar órdenes
        private readonly IUserRepository _userRepository;  // Repositorio de usuarios para obtener datos del usuario actual

        /// <summary>
        /// Constructor que recibe los servicios necesarios para este controlador.
        /// </summary>
        /// <param name="orderService">Servicio para manejar órdenes.</param>
        /// <param name="userRepository">Repositorio de usuarios.</param>
        public OrderController(IOrderService orderService, IUserRepository userRepository)
        {
            _orderService = orderService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Endpoint para que el administrador obtenga todas las órdenes con opciones de paginación, búsqueda y ordenamiento.
        /// Solo los administradores pueden acceder a este endpoint.
        /// </summary>
        /// <param name="pageNumber">Número de página (opcional, valor por defecto 1).</param>
        /// <param name="pageSize">Cantidad de órdenes por página (opcional, valor por defecto 10).</param>
        /// <param name="searchTerm">Término de búsqueda (opcional).</param>
        /// <param name="sortOrder">Orden de clasificación (opcional).</param>
        /// <returns>Una lista de órdenes con los parámetros solicitados.</returns>
        /// <response code="200">Devuelve las órdenes encontradas.</response>
        /// <response code="500">Error interno del servidor si ocurre una excepción.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]  // Solo los administradores pueden acceder a este endpoint
        public async Task<IActionResult> GetOrders(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? sortOrder = null)
        {
            try
            {
                // Llamada al servicio para obtener las órdenes con los parámetros proporcionados
                var result = await _orderService.GetOrdersAsync(pageNumber, pageSize, searchTerm, sortOrder);
                return Ok(result);  // Retorna las órdenes encontradas
            }
            catch (Exception ex)
            {
                // En caso de error, se retorna un mensaje de error con el código 500
                return StatusCode(500, new { message = "Error al obtener las órdenes", error = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint para que un usuario obtenga sus propias órdenes con paginación.
        /// Solo los usuarios con el rol "Customer" pueden acceder a este endpoint.
        /// </summary>
        /// <param name="pageNumber">Número de página (opcional, valor por defecto 1).</param>
        /// <param name="pageSize">Cantidad de órdenes por página (opcional, valor por defecto 10).</param>
        /// <returns>Una lista de las órdenes del usuario actual.</returns>
        /// <response code="200">Devuelve las órdenes del usuario encontrado.</response>
        /// <response code="404">Si el usuario no es encontrado.</response>
        /// <response code="403">Si el usuario intenta acceder a las órdenes de otro usuario.</response>
        /// <response code="500">Error interno del servidor si ocurre una excepción.</response>
        [HttpGet("orders")]
        [Authorize(Roles = "Customer")]  // Solo los usuarios con el rol "Customer" pueden acceder
        public async Task<IActionResult> GetUserOrders(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // Obtener el usuario actual usando el repositorio
                var user = await _userRepository.GetCurrentUserAsync();

                if (user == null)
                {
                    return NotFound(new { message = "Usuario no encontrado." });  // Si no se encuentra el usuario, se retorna un error 404
                }

                // Verificar que el usuario autenticado solo pueda acceder a sus propias órdenes
                if (user.Id != long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))  // Comparar el ID del usuario con el ID del claim en el token
                {
                    return Forbid("No tienes permiso para acceder a las órdenes de otro usuario.");  // Si no coincide, se prohíbe el acceso
                }

                // Obtener las órdenes del usuario actual
                var result = await _orderService.GetOrdersByUserIdAsync(user.Id, pageNumber, pageSize);
                return Ok(result);  // Retorna las órdenes encontradas para el usuario
            }
            catch (Exception ex)
            {
                // En caso de error, se retorna un mensaje de error con el código 500
                return StatusCode(500, new { message = "Error al obtener las órdenes", error = ex.Message });
            }
        }
    }
}
