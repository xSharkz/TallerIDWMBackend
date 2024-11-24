using System.Collections.Generic;
using System.Threading.Tasks;
using TallerIDWMBackend.Models;

namespace TallerIDWMBackend.Interfaces
{
    /// <summary>
    /// Interfaz para gestionar las operaciones de acceso a datos relacionadas con las órdenes.
    /// Proporciona métodos para obtener todas las órdenes, obtener el total de órdenes, 
    /// y obtener las órdenes de un usuario específico.
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Obtiene una lista paginada de órdenes, con la opción de filtrar por término de búsqueda 
        /// y ordenar los resultados.
        /// </summary>
        /// <param name="pageNumber">Número de página para la paginación (empieza desde 1).</param>
        /// <param name="pageSize">Número de elementos por página.</param>
        /// <param name="searchTerm">Término de búsqueda para filtrar las órdenes (opcional).</param>
        /// <param name="sortOrder">Orden de clasificación de los resultados, puede ser ascendente o descendente (opcional).</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene una lista de órdenes.</returns>
        Task<IEnumerable<Order>> GetAllOrdersAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null);

        /// <summary>
        /// Obtiene el total de órdenes disponibles, con la opción de filtrar por término de búsqueda.
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda para filtrar las órdenes (opcional).</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el número total de órdenes.</returns>
        Task<int> GetTotalOrdersCountAsync(string? searchTerm = null);

        /// <summary>
        /// Obtiene las órdenes de un usuario específico.
        /// </summary>
        /// <param name="userId">ID del usuario para el cual se obtendrán las órdenes.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene una lista de órdenes del usuario.</returns>
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(long userId);
    }
}
