using TallerIDWMBackend.DTOs.User;
using TallerIDWMBackend.Models;

namespace TallerIDWMBackend.Interfaces
{
    /// <summary>
    /// Interfaz para gestionar las operaciones relacionadas con los usuarios en el repositorio.
    /// Permite obtener, actualizar, agregar y gestionar el estado de los usuarios, así como realizar consultas con paginación y búsqueda.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Obtiene el usuario actual que está autenticado o en sesión.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el usuario actual.</returns>
        Task<User> GetCurrentUserAsync();

        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        /// <param name="id">El ID del usuario a obtener.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el usuario con el ID especificado.</returns>
        Task<User> GetUserByIdAsync(long id);

        /// <summary>
        /// Actualiza los datos de un usuario en el repositorio.
        /// </summary>
        /// <param name="user">El usuario con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task UpdateUserAsync(User user);

        /// <summary>
        /// Obtiene un usuario por su dirección de correo electrónico.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario a obtener.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el usuario con el correo electrónico especificado.</returns>
        Task<User> GetUserByEmailAsync(string email);

        /// <summary>
        /// Verifica si el correo electrónico o el RUT ya están registrados en el sistema.
        /// </summary>
        /// <param name="email">El correo electrónico a verificar.</param>
        /// <param name="rut">El RUT a verificar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado es un valor booleano que indica si el correo electrónico o RUT ya están registrados.</returns>
        Task<bool> IsEmailOrRutRegisteredAsync(string email, string rut);

        /// <summary>
        /// Agrega un nuevo usuario al repositorio.
        /// </summary>
        /// <param name="user">El usuario a agregar.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task AddUserAsync(User user);

        /// <summary>
        /// Guarda los cambios realizados en el repositorio.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task SaveChangesAsync();

        /// <summary>
        /// Obtiene una lista de usuarios con paginación y búsqueda.
        /// </summary>
        /// <param name="page">El número de página para la paginación.</param>
        /// <param name="pageSize">El número de usuarios por página.</param>
        /// <param name="searchQuery">El término de búsqueda para filtrar los usuarios.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene una lista de usuarios paginados.</returns>
        Task<PaginatedResponseDto<User>> GetPaginatedUsersAsync(int page, int pageSize, string searchQuery);

        /// <summary>
        /// Actualiza el estado de un usuario (habilitado/deshabilitado).
        /// </summary>
        /// <param name="userId">El ID del usuario cuyo estado se actualizará.</param>
        /// <param name="isEnabled">El nuevo estado del usuario. True si el usuario está habilitado, false si está deshabilitado.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task UpdateUserStatusAsync(long userId, bool isEnabled);
    }
}
