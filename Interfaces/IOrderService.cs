using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TallerIDWMBackend.DTOs.Order;
using TallerIDWMBackend.DTOs.User;
using TallerIDWMBackend.Models;

namespace TallerIDWMBackend.Interfaces
{
    /// <summary>
    /// Interfaz para gestionar las operaciones de servicio relacionadas con las órdenes.
    /// Proporciona métodos para obtener órdenes paginadas, tanto generales como por usuario.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Obtiene una lista de órdenes paginadas, con la opción de filtrar por término de búsqueda 
        /// y ordenar los resultados.
        /// </summary>
        /// <param name="pageNumber">Número de página para la paginación (empieza desde 1).</param>
        /// <param name="pageSize">Número de elementos por página.</param>
        /// <param name="searchTerm">Término de búsqueda para filtrar las órdenes (opcional).</param>
        /// <param name="sortOrder">Orden de clasificación de los resultados, puede ser ascendente o descendente (opcional).</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene una respuesta paginada con órdenes.</returns>
        Task<PaginatedResponseDto<OrderDto>> GetOrdersAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null);

        /// <summary>
        /// Obtiene las órdenes de un usuario específico en una lista paginada.
        /// </summary>
        /// <param name="userId">ID del usuario para el cual se obtendrán las órdenes.</param>
        /// <param name="pageNumber">Número de página para la paginación (empieza desde 1).</param>
        /// <param name="pageSize">Número de elementos por página.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene una respuesta paginada con las órdenes del usuario.</returns>
        Task<PaginatedResponseDto<OrderDto>> GetOrdersByUserIdAsync(long userId, int pageNumber, int pageSize);
    }
}
