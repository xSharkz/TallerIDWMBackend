using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TallerIDWMBackend.Interfaces;

namespace TallerIDWMBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? sortOrder = null)
        {
            var result = await _orderService.GetOrdersAsync(pageNumber, pageSize, searchTerm, sortOrder);
            return Ok(result);
        }
    }
}