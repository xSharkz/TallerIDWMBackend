using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TallerIDWMBackend.Models;

namespace TallerIDWMBackend.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetPagedProductsAsync(string searchQuery, string type, string sortOrder, int pageNumber, int pageSize);
        Task<int> GetTotalProductsAsync(string searchQuery, string type);
        Task<Product> GetProductByIdAsync(int id);
    }
}