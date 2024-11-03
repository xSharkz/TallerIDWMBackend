using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TallerIDWMBackend.Models;

namespace TallerIDWMBackend.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null);
        Task<int> GetTotalOrdersCountAsync(string? searchTerm = null);
    }
}