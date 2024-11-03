using System.Collections.Generic;
using System.Threading.Tasks;
using TallerIDWMBackend.Models;

namespace TallerIDWMBackend.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null);
        Task<int> GetTotalOrdersCountAsync(string? searchTerm = null);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(long userId); // Método para obtener órdenes por usuario
    }
}
