using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TallerIDWMBackend.Models;

namespace TallerIDWMBackend.Interfaces
{
    /// <summary>
    /// Interfaz para gestionar las operaciones relacionadas con los productos en el repositorio.
    /// Permite obtener, agregar, actualizar y eliminar productos, así como realizar consultas con paginación y filtrado.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Obtiene los productos con paginación, filtrado por términos de búsqueda, tipo y orden de clasificación.
        /// </summary>
        /// <param name="searchQuery">El término de búsqueda para filtrar los productos por nombre o características.</param>
        /// <param name="type">El tipo de producto a filtrar (por ejemplo, "Poleras", "Gorros").</param>
        /// <param name="sortOrder">El orden en que se deben mostrar los productos (por ejemplo, "asc" para ascendente, "desc" para descendente).</param>
        /// <param name="pageNumber">El número de página para la paginación.</param>
        /// <param name="pageSize">El número de productos por página.</param>
        /// <param name="includeOutOfStock">Indica si se deben incluir los productos fuera de stock (valor por defecto: false).</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene una lista de productos paginados.</returns>
        Task<IEnumerable<Product>> GetPagedProductsAsync(string searchQuery, string type, string sortOrder, int pageNumber, int pageSize, bool includeOutOfStock = false);

        /// <summary>
        /// Obtiene el total de productos disponibles según los filtros de búsqueda y tipo.
        /// </summary>
        /// <param name="searchQuery">El término de búsqueda para filtrar los productos.</param>
        /// <param name="type">El tipo de producto a filtrar.</param>
        /// <param name="includeOutOfStock">Indica si se deben contar los productos fuera de stock (valor por defecto: false).</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el número total de productos que cumplen con los filtros.</returns>
        Task<int> GetTotalProductsAsync(string searchQuery, string type, bool includeOutOfStock = false);

        /// <summary>
        /// Obtiene un producto por su ID.
        /// </summary>
        /// <param name="id">El ID del producto a buscar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el producto con el ID especificado.</returns>
        Task<Product> GetProductByIdAsync(long id);

        /// <summary>
        /// Verifica si un producto con el mismo nombre y tipo ya existe en el repositorio.
        /// </summary>
        /// <param name="name">El nombre del producto.</param>
        /// <param name="type">El tipo de producto.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado es un valor booleano que indica si el producto existe o no.</returns>
        Task<bool> ProductExistsAsync(string name, string type);

        /// <summary>
        /// Agrega un nuevo producto al repositorio.
        /// </summary>
        /// <param name="product">El producto a agregar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el producto agregado, incluyendo su ID generado.</returns>
        Task<Product> AddProductAsync(Product product);

        /// <summary>
        /// Actualiza los detalles de un producto existente en el repositorio.
        /// </summary>
        /// <param name="id">El ID del producto a actualizar.</param>
        /// <param name="updatedProduct">El objeto con los nuevos detalles del producto.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el producto actualizado.</returns>
        Task<Product> UpdateProductAsync(long id, Product updatedProduct);

        /// <summary>
        /// Elimina un producto del repositorio por su ID.
        /// </summary>
        /// <param name="id">El ID del producto a eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task DeleteProductAsync(long id);
    }
}
