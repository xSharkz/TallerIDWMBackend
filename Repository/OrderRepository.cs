using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TallerIDWMBackend.Data;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.Models;

namespace TallerIDWMBackend.Repository
{
    /// <summary>
    /// Repositorio para manejar las operaciones de acceso a datos de las órdenes.
    /// Implementa la interfaz <see cref="IOrderRepository"/> para interactuar con la base de datos de órdenes.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// Constructor que inicializa el repositorio con el contexto de la base de datos.
        /// </summary>
        /// <param name="context">El contexto de la base de datos que maneja las operaciones de Entity Framework.</param>
        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }
         /// <summary>
        /// Obtiene todas las órdenes de la base de datos con paginación, filtrado y ordenación.
        /// </summary>
        /// <param name="pageNumber">El número de página para la paginación.</param>
        /// <param name="pageSize">El tamaño de cada página.</param>
        /// <param name="searchTerm">El término de búsqueda para filtrar las órdenes por nombre de usuario (opcional).</param>
        /// <param name="sortOrder">El orden en el que se deben devolver las órdenes (opcional, por ejemplo, "date_desc" para ordenar por fecha descendente).</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado es una lista de órdenes.</returns>
        public async Task<IEnumerable<Order>> GetAllOrdersAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null)
        {
            var query = _context.Orders.Include(o => o.OrderItems).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var userQuery = _context.Users
                    .Where(u => u.Name.Contains(searchTerm))
                    .Select(u => u.Id);
                query = query.Where(o => userQuery.Contains(o.UserId));
            }

            query = sortOrder switch
            {
                "date_desc" => query.OrderByDescending(o => o.OrderDate),
                _ => query.OrderBy(o => o.OrderDate),
            };

            return await query.Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();
        }
        /// <summary>
        /// Obtiene el número total de órdenes en la base de datos con la opción de filtrar por nombre de usuario.
        /// </summary>
        /// <param name="searchTerm">El término de búsqueda para filtrar las órdenes por nombre de usuario (opcional).</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado es el número total de órdenes.</returns>
        public async Task<int> GetTotalOrdersCountAsync(string? searchTerm = null)
        {
            var query = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var userQuery = _context.Users
                    .Where(u => u.Name.Contains(searchTerm))
                    .Select(u => u.Id);
                query = query.Where(o => userQuery.Contains(o.UserId));
            }

            return await query.CountAsync();
        }
        /// <summary>
        /// Obtiene todas las órdenes asociadas a un usuario específico por su ID.
        /// </summary>
        /// <param name="userId">El ID del usuario cuyas órdenes se desean obtener.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado es una lista de órdenes del usuario.</returns>
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(long userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }
    }
}
