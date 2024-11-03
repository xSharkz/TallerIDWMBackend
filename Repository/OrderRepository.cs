using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TallerIDWMBackend.Data;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.Models;

namespace TallerIDWMBackend.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

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

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(long userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }
    }
}
