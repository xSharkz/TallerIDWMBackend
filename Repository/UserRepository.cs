using Microsoft.EntityFrameworkCore;
using TallerIDWMBackend.Data;
using TallerIDWMBackend.DTOs.User;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.Models;
using TallerIDWMBackend.Services;

public class UserRepository : IUserRepository
{
    /// <summary>
    /// Repositorio para manejar las operaciones de acceso a datos de los usuarios.
    /// Implementa la interfaz <see cref="IUserRepository"/> para interactuar con la base de datos de usuarios.
    /// </summary>
    private readonly ApplicationDbContext _dataContext;
    private readonly UserContextService _userContextService;
    /// <summary>
    /// Constructor que inicializa el repositorio con el contexto de la base de datos y el servicio de contexto del usuario.
    /// </summary>
    /// <param name="dataContext">El contexto de la base de datos que maneja las operaciones de Entity Framework.</param>
    /// <param name="userContextService">El servicio que obtiene el contexto del usuario desde los claims.</param>
    public UserRepository(ApplicationDbContext dataContext, UserContextService userContextService)
    {
        _dataContext = dataContext;
        _userContextService = userContextService;
    }
    /// <summary>
    /// Obtiene el usuario actualmente autenticado a partir de los claims.
    /// </summary>
    /// <returns>Una tarea que representa la operación asincrónica. El resultado es el usuario actual.</returns>
    public async Task<User> GetCurrentUserAsync()
    {
        long userId = _userContextService.GetUserIdFromClaims();
        return await GetUserByIdAsync(userId);
    }
    /// <summary>
    /// Obtiene un usuario por su ID.
    /// </summary>
    /// <param name="id">El ID del usuario que se desea obtener.</param>
    /// <returns>Una tarea que representa la operación asincrónica. El resultado es el usuario con el ID proporcionado.</returns>
    public async Task<User> GetUserByIdAsync(long id)
    {
        return await _dataContext.Users.FindAsync(id);
    }
    /// <summary>
    /// Actualiza los detalles de un usuario existente.
    /// </summary>
    /// <param name="user">El objeto con los nuevos detalles del usuario.</param>
    /// <returns>Una tarea que representa la operación asincrónica.</returns>
    public async Task UpdateUserAsync(User user)
    {
        _dataContext.Users.Update(user);
        await _dataContext.SaveChangesAsync();
    }
    /// <summary>
    /// Obtiene un usuario por su dirección de correo electrónico.
    /// </summary>
    /// <param name="email">La dirección de correo electrónico del usuario.</param>
    /// <returns>Una tarea que representa la operación asincrónica. El resultado es el usuario con la dirección de correo electrónico proporcionada.</returns>
    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _dataContext.Users.SingleOrDefaultAsync(u => u.Email == email);
    }
    /// <summary>
    /// Verifica si el correo electrónico o el RUT del usuario ya están registrados en la base de datos.
    /// </summary>
    /// <param name="email">La dirección de correo electrónico del usuario.</param>
    /// <param name="rut">El RUT del usuario.</param>
    /// <returns>Una tarea que representa la operación asincrónica. El resultado es verdadero si el correo electrónico o el RUT ya están registrados, de lo contrario falso.</returns>
    public async Task<bool> IsEmailOrRutRegisteredAsync(string email, string rut)
    {
        return await _dataContext.Users.AnyAsync(u => u.Email == email || u.Rut == rut);
    }
    /// <summary>
    /// Agrega un nuevo usuario a la base de datos.
    /// </summary>
    /// <param name="user">El usuario que se desea agregar.</param>
    /// <returns>Una tarea que representa la operación asincrónica.</returns>
    public async Task AddUserAsync(User user)
    {
        _dataContext.Users.Add(user);
        await _dataContext.SaveChangesAsync();
    }
    /// <summary>
    /// Guarda los cambios realizados en la base de datos.
    /// </summary>
    /// <returns>Una tarea que representa la operación asincrónica.</returns>
    public async Task SaveChangesAsync()
    {
        await _dataContext.SaveChangesAsync();
    }
    /// <summary>
    /// Obtiene los usuarios paginados, con la opción de filtrarlos por un término de búsqueda.
    /// </summary>
    /// <param name="page">El número de página de los resultados.</param>
    /// <param name="pageSize">El número de usuarios por página.</param>
    /// <param name="searchQuery">El término de búsqueda para filtrar usuarios por nombre o correo electrónico (opcional).</param>
    /// <returns>Una tarea que representa la operación asincrónica. El resultado es una respuesta paginada de usuarios.</returns>
    public async Task<PaginatedResponseDto<User>> GetPaginatedUsersAsync(int page, int pageSize, string searchQuery)
    {
        var query = _dataContext.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(u => u.Name.Contains(searchQuery) || u.Email.Contains(searchQuery));
        }

        int totalUsers = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponseDto<User>
        {
            Items = users,
            TotalPages = totalPages,
            CurrentPage = page
        };
    }
    /// <summary>
    /// Actualiza el estado de habilitación de un usuario.
    /// </summary>
    /// <param name="userId">El ID del usuario cuyo estado se desea actualizar.</param>
    /// <param name="isEnabled">El nuevo estado de habilitación del usuario (verdadero si está habilitado, falso si está deshabilitado).</param>
    /// <returns>Una tarea que representa la operación asincrónica.</returns>
    public async Task UpdateUserStatusAsync(long userId, bool isEnabled)
    {
        var user = await _dataContext.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsEnabled = isEnabled;
            _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();
        }
    }
}
