using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TallerIDWMBackend.Models;

namespace TallerIDWMBackend.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetPagedProductsAsync(string searchQuery, string type, string sortOrder, int pageNumber, int pageSize, bool includeOutOfStock = false);
        Task<int> GetTotalProductsAsync(string searchQuery, string type, bool includeOutOfStock = false);
        Task<Product> GetProductByIdAsync(long id);

        Task<bool> ProductExistsAsync(string name, string type);
        Task<Product> AddProductAsync(Product product);
        Task<Product> UpdateProductAsync(long id, Product updatedProduct);
        Task DeleteProductAsync(long id);

    }
}