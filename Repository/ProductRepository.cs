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

        public async Task<Product> GetProductByIdAsync(long id)
        {
            var product = await _dataContext.Products.FindAsync(id);

            if (product == null)
            {
                throw new NullReferenceException($"Product with ID {id} not found.");
            }

            return product;
        }

        public async Task<bool> ProductExistsAsync(string name, string type)
        {
            return await _dataContext.Products.AnyAsync(p => p.Name == name && p.Type == type);
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            if (await ProductExistsAsync(product.Name, product.Type))
            {
                throw new InvalidOperationException("Ya existe un producto con el mismo nombre y tipo.");
            }

            _dataContext.Products.Add(product);
            await _dataContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(long id, Product updatedProduct)
        {
            var existingProduct = await GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                throw new NullReferenceException($"Producto con ID {id} no encontrado.");
            }

            if (await ProductExistsAsync(updatedProduct.Name, updatedProduct.Type) &&
                (existingProduct.Name != updatedProduct.Name || existingProduct.Type != updatedProduct.Type))
            {
                throw new InvalidOperationException("Ya existe otro producto con el mismo nombre y tipo.");
            }

            existingProduct.Name = updatedProduct.Name;
            existingProduct.Type = updatedProduct.Type;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.StockQuantity = updatedProduct.StockQuantity;
            existingProduct.ImageUrl = updatedProduct.ImageUrl;
            existingProduct.PublicId = updatedProduct.PublicId;

            _dataContext.Products.Update(existingProduct);
            await _dataContext.SaveChangesAsync();
            return existingProduct;
        }

        public async Task DeleteProductAsync(long id)
        {
            var product = await GetProductByIdAsync(id);
            if (product == null)
            {
                throw new NullReferenceException($"Producto con ID {id} no encontrado.");
            }

            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
        }

    }
}