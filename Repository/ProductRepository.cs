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
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dataContext;

        public ProductRepository(ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IEnumerable<Product>> GetPagedProductsAsync(string searchQuery, string type, string sortOrder, int pageNumber, int pageSize)
        {
            // Filtrar productos con stock mayor que 0
            var productsQuery = _dataContext.Products.Where(p => p.StockQuantity > 0).AsQueryable();

            // Filtro de búsqueda por nombre del producto
            if (!string.IsNullOrEmpty(searchQuery))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchQuery));
            }

            // Filtrar por tipo de producto
            if (!string.IsNullOrEmpty(type))
            {
                productsQuery = productsQuery.Where(p => p.Type == type);
            }

            // Ordenar por precio ascendente o descendente
            switch (sortOrder.ToLower())
            {
                case "desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
                default:
                    productsQuery = productsQuery.OrderBy(p => p.Price);
                    break;
            }

            // Paginación
            return await productsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalProductsAsync(string searchQuery, string type)
        {
            var productsQuery = _dataContext.Products.Where(p => p.StockQuantity > 0).AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchQuery));
            }

            if (!string.IsNullOrEmpty(type))
            {
                productsQuery = productsQuery.Where(p => p.Type == type);
            }

            return await productsQuery.CountAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _dataContext.Products.FindAsync(id);
        }
    }
}