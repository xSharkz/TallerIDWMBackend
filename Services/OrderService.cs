using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TallerIDWMBackend.DTOs.User;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.Models;

namespace TallerIDWMBackend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<PaginatedResponseDto<Order>> GetOrdersAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null)
        {
            var totalCount = await _orderRepository.GetTotalOrdersCountAsync(searchTerm);
            var orders = await _orderRepository.GetAllOrdersAsync(pageNumber, pageSize, searchTerm, sortOrder);

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PaginatedResponseDto<Order>
            {
                Items = orders.ToList(),
                TotalPages = totalPages,
                CurrentPage = pageNumber
            };
        }
    }
}