using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TallerIDWMBackend.DTOs.User;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.Repository;

namespace TallerIDWMBackend.Controllers
{
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con los usuarios, como la edición de perfil,
    /// cambio de contraseña, obtención de clientes y actualización del estado de los usuarios.
    /// </summary>
    [ApiController] // Atributo que indica que esta clase es un controlador de API
    [Route("api/[controller]")] // Define la ruta base para este controlador
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository; // Interfaz del repositorio de usuarios

        /// <summary>
        /// Constructor que inyecta la dependencia del repositorio de usuarios.
        /// </summary>
        /// <param name="userRepository">Repositorio de usuarios que será utilizado para acceder a los datos del usuario.</param>
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Permite editar el perfil del usuario.
        /// Solo accesible para usuarios con los roles de Customer o Admin.
        /// </summary>
        /// <param name="editProfileDto">Objeto que contiene los nuevos datos del perfil del usuario.</param>
        /// <returns>Resultado de la operación con mensaje de éxito o error.</returns>
        [HttpPut("edit-profile")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> EditProfile([FromForm] EditProfileDto editProfileDto)
        {
            var user = await _userRepository.GetCurrentUserAsync();

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            // Mantener el nombre actual si no se proporciona un nuevo valor en editProfileDto
            user.Name = editProfileDto.Name ?? user.Name;
            user.BirthDate = editProfileDto.BirthDate ?? user.BirthDate;
            user.Gender = editProfileDto.Gender ?? user.Gender;

            await _userRepository.UpdateUserAsync(user);

            return Ok(new { message = "Perfil actualizado exitosamente." });
        }

        /// <summary>
        /// Permite cambiar la contraseña del usuario.
        /// Solo accesible para usuarios con los roles de Customer o Admin.
        /// </summary>
        /// <param name="changePasswordDto">Objeto que contiene la contraseña actual y la nueva contraseña.</param>
        /// <returns>Resultado de la operación con mensaje de éxito o error.</returns>
        [HttpPut("change-password")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetCurrentUserAsync();

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return Unauthorized(new { message = "Contraseña actual incorrecta." });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _userRepository.UpdateUserAsync(user);

            return Ok(new { message = "Contraseña cambiada exitosamente." });
        }

        /// <summary>
        /// Obtiene una lista de todos los clientes registrados.
        /// Solo accesible para usuarios con el rol de Admin.
        /// </summary>
        /// <param name="page">Número de página para paginación (valor por defecto: 1).</param>
        /// <param name="pageSize">Número de elementos por página (valor por defecto: 10).</param>
        /// <param name="searchQuery">Texto para buscar en los usuarios (valor por defecto: vacío).</param>
        /// <returns>Lista paginada de los usuarios con sus detalles.</returns>
        [HttpGet("customers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCustomers([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchQuery = "")
        {
            var paginatedUsers = await _userRepository.GetPaginatedUsersAsync(page, pageSize, searchQuery);

            var userDtos = paginatedUsers.Items.Select(user => new UserDto
            {
                Id = user.Id,
                Rut = user.Rut,
                Name = user.Name,
                Email = user.Email,
                Gender = user.Gender,
                IsEnabled = user.IsEnabled
            }).ToList();

            return Ok(new PaginatedResponseDto<UserDto>
            {
                Items = userDtos,
                TotalPages = paginatedUsers.TotalPages,
                CurrentPage = paginatedUsers.CurrentPage
            });
        }

        /// <summary>
        /// Actualiza el estado de un usuario (habilitado o deshabilitado).
        /// Solo accesible para usuarios con el rol de Admin.
        /// </summary>
        /// <param name="updateUserStatusDto">Objeto que contiene el ID del usuario y el nuevo estado (habilitado o deshabilitado).</param>
        /// <returns>Resultado de la operación con mensaje de éxito o error.</returns>
        [HttpPut("update-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserStatus([FromForm] UpdateUserStatusDto updateUserStatusDto)
        {
            var user = await _userRepository.GetUserByIdAsync(updateUserStatusDto.UserId);

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            await _userRepository.UpdateUserStatusAsync(updateUserStatusDto.UserId, updateUserStatusDto.IsEnabled);
            var status = updateUserStatusDto.IsEnabled ? "habilitada" : "deshabilitada";

            return Ok(new { message = $"Cuenta de usuario {status} exitosamente." });
        }
    }
}
