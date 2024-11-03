using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TallerIDWMBackend.DTOs.Order;
using TallerIDWMBackend.DTOs.User;
using TallerIDWMBackend.Models;

namespace TallerIDWMBackend.Interfaces
{
    public interface IOrderService
    {
        Task<PaginatedResponseDto<OrderDto>> GetOrdersAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null);
    }
}