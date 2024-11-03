using System;
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
            var query = _context.Orders.Include(o => o.User).AsQueryable();

            // Filtrar por nombre del cliente
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(o => o.User.Name.Contains(searchTerm));
            }

            // Ordenar por fecha
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
            var query = _context.Orders.Include(o => o.User).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(o => o.User.Name.Contains(searchTerm));
            }

            return await query.CountAsync();
        }
    }
}