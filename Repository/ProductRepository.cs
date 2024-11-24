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
        /// <summary>
        /// Repositorio para manejar las operaciones de acceso a datos de los productos.
        /// Implementa la interfaz <see cref="IProductRepository"/> para interactuar con la base de datos de productos.
        /// </summary>
        private readonly ApplicationDbContext _dataContext;
        /// <summary>
        /// Constructor que inicializa el repositorio con el contexto de la base de datos.
        /// </summary>
        /// <param name="dataContext">El contexto de la base de datos que maneja las operaciones de Entity Framework.</param>
        public ProductRepository(ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
        }
        /// <summary>
        /// Obtiene productos con paginación, filtrado y ordenación.
        /// </summary>
        /// <param name="searchQuery">El término de búsqueda para filtrar productos por nombre (opcional).</param>
        /// <param name="type">El tipo de producto para filtrar (opcional).</param>
        /// <param name="sortOrder">El orden en el que se deben devolver los productos, por ejemplo, "asc" o "desc".</param>
        /// <param name="pageNumber">El número de página para la paginación.</param>
        /// <param name="pageSize">El tamaño de cada página.</param>
        /// <param name="includeOutOfStock">Indica si los productos sin stock deben incluirse en el resultado.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado es una lista de productos.</returns>
        public async Task<IEnumerable<Product>> GetPagedProductsAsync(string searchQuery, string type, string sortOrder, int pageNumber, int pageSize, bool includeOutOfStock = false)
        {
            var productsQuery = _dataContext.Products.AsQueryable();

            // Filtrar productos con stock > 0 si no se incluye out of stock
            if (!includeOutOfStock)
            {
                productsQuery = productsQuery.Where(p => p.StockQuantity > 0);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchQuery));
            }

            if (!string.IsNullOrEmpty(type))
            {
                productsQuery = productsQuery.Where(p => p.Type == type);
            }

            // Ordenamiento
            productsQuery = sortOrder.ToLower() == "desc"
                ? productsQuery.OrderByDescending(p => p.Price)
                : productsQuery.OrderBy(p => p.Price);

            // Paginación
            return await productsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene el número total de productos, con la opción de filtrar por nombre y tipo.
        /// </summary>
        /// <param name="searchQuery">El término de búsqueda para filtrar productos por nombre (opcional).</param>
        /// <param name="type">El tipo de producto para filtrar (opcional).</param>
        /// <param name="includeOutOfStock">Indica si los productos sin stock deben incluirse en el resultado.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado es el número total de productos.</returns>
        public async Task<int> GetTotalProductsAsync(string searchQuery, string type, bool includeOutOfStock = false)
        {
            var productsQuery = _dataContext.Products.AsQueryable();

            if (!includeOutOfStock)
            {
                productsQuery = productsQuery.Where(p => p.StockQuantity > 0);
            }

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
        /// <summary>
        /// Obtiene un producto por su ID.
        /// </summary>
        /// <param name="id">El ID del producto que se desea obtener.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado es el producto con el ID proporcionado.</returns>
        /// <exception cref="NullReferenceException">Se lanza si el producto no se encuentra.</exception>
        public async Task<Product> GetProductByIdAsync(long id)
        {
            var product = await _dataContext.Products.FindAsync(id);

            if (product == null)
            {
                throw new NullReferenceException($"Product with ID {id} not found.");
            }

            return product;
        }
        /// <summary>
        /// Verifica si existe un producto con el mismo nombre y tipo.
        /// </summary>
        /// <param name="name">El nombre del producto.</param>
        /// <param name="type">El tipo del producto.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado es verdadero si el producto existe, falso en caso contrario.</returns>
        public async Task<bool> ProductExistsAsync(string name, string type)
        {
            return await _dataContext.Products.AnyAsync(p => p.Name == name && p.Type == type);
        }
        /// <summary>
        /// Agrega un nuevo producto a la base de datos.
        /// </summary>
        /// <param name="product">El producto a agregar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado es el producto agregado.</returns>
        /// <exception cref="InvalidOperationException">Se lanza si ya existe un producto con el mismo nombre y tipo.</exception>
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
        /// <summary>
        /// Actualiza los detalles de un producto existente.
        /// </summary>
        /// <param name="id">El ID del producto que se desea actualizar.</param>
        /// <param name="updatedProduct">El objeto con los nuevos detalles del producto.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado es el producto actualizado.</returns>
        /// <exception cref="NullReferenceException">Se lanza si el producto no se encuentra.</exception>
        /// <exception cref="InvalidOperationException">Se lanza si ya existe otro producto con el mismo nombre y tipo.</exception>
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
        /// <summary>
        /// Elimina un producto de la base de datos.
        /// </summary>
        /// <param name="id">El ID del producto que se desea eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        /// <exception cref="NullReferenceException">Se lanza si el producto no se encuentra.</exception>
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