using System;
using System.Linq;
using System.Threading.Tasks;
using TallerIDWMBackend.DTOs.Order;
using TallerIDWMBackend.DTOs.User;
using TallerIDWMBackend.Interfaces;

namespace TallerIDWMBackend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<PaginatedResponseDto<OrderDto>> GetOrdersAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null)
        {
            var totalCount = await _orderRepository.GetTotalOrdersCountAsync(searchTerm);
            var orders = await _orderRepository.GetAllOrdersAsync(pageNumber, pageSize, searchTerm, sortOrder);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var orderDtos = orders.Select(order => new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
                DeliveryAddress = order.DeliveryAddress,
                OrderItems = order.OrderItems.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            }).ToList();

            return new PaginatedResponseDto<OrderDto>
            {
                Items = orderDtos,
                TotalPages = totalPages,
                CurrentPage = pageNumber
            };
        }

        public async Task<PaginatedResponseDto<OrderDto>> GetOrdersByUserIdAsync(long userId, int pageNumber, int pageSize)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            var totalOrders = orders.Count();
            var paginatedOrders = orders.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);

            var orderDtos = paginatedOrders.Select(order => new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
                DeliveryAddress = order.DeliveryAddress,
                OrderItems = order.OrderItems.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            }).ToList();

            return new PaginatedResponseDto<OrderDto>
            {
                Items = orderDtos,
                TotalPages = totalPages,
                CurrentPage = pageNumber
            };
        }
    }
}
