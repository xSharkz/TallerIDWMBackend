using System;
using System.Linq;
using System.Threading.Tasks;
using TallerIDWMBackend.DTOs.Order;
using TallerIDWMBackend.DTOs.User;
using TallerIDWMBackend.Interfaces;

namespace TallerIDWMBackend.Services
{
    /// <summary>
    /// Servicio que maneja la lógica de negocio relacionada con las órdenes de compra.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="OrderService"/> con el repositorio de órdenes.
        /// </summary>
        /// <param name="orderRepository">El repositorio de órdenes que proporciona acceso a los datos de las órdenes.</param>
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        /// <summary>
        /// Obtiene las órdenes de compra paginadas, con la opción de buscar por un término y ordenar los resultados.
        /// </summary>
        /// <param name="pageNumber">El número de la página que se desea obtener.</param>
        /// <param name="pageSize">El número de órdenes por página.</param>
        /// <param name="searchTerm">Término de búsqueda (opcional), utilizado para filtrar las órdenes por el nombre de usuario o dirección.</param>
        /// <param name="sortOrder">Orden de clasificación (opcional) para las órdenes. Puede ser un valor que determine el orden de los resultados.</param>
        /// <returns>Un objeto <see cref="PaginatedResponseDto{OrderDto}"/> con las órdenes de la página solicitada y la información de paginación.</returns>

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
        /// <summary>
        /// Obtiene las órdenes de compra paginadas de un usuario específico.
        /// </summary>
        /// <param name="userId">El ID del usuario cuyas órdenes se desean obtener.</param>
        /// <param name="pageNumber">El número de la página que se desea obtener.</param>
        /// <param name="pageSize">El número de órdenes por página.</param>
        /// <returns>Un objeto <see cref="PaginatedResponseDto{OrderDto}"/> con las órdenes de la página solicitada para el usuario específico.</returns>
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
