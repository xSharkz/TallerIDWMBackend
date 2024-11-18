using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TallerIDWMBackend.DTOs.User;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.Repository;

namespace TallerIDWMBackend.Controllers
{
    [ApiController] // Atributo que indica que esta clase es un controlador de API
    [Route("api/[controller]")] // Define la ruta base para este controlador
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository; // Interfaz del repositorio de usuarios

        // Constructor que inyecta la dependencia del repositorio de usuarios
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // 1. Editar perfil de usuario
        [HttpPut("edit-profile")] // Ruta para actualizar el perfil
        [Authorize(Roles = "Customer,Admin")] // Requiere estar autenticado y tener el rol de Customer o Admin
        public async Task<IActionResult> EditProfile([FromForm] EditProfileDto editProfileDto)
        {
            // Obtener el usuario actual usando el método GetCurrentUserAsync del repositorio
            var user = await _userRepository.GetCurrentUserAsync();

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." }); // Si el usuario no se encuentra, retorna 404
            }

            // Mantener el nombre actual si no se proporciona un nuevo valor en editProfileDto
            user.Name = editProfileDto.Name ?? user.Name;
            user.BirthDate = editProfileDto.BirthDate ?? user.BirthDate;
            user.Gender = editProfileDto.Gender ?? user.Gender;

            // Actualizar los datos del usuario en la base de datos
            await _userRepository.UpdateUserAsync(user);

            return Ok(new { message = "Perfil actualizado exitosamente." }); // Respuesta exitosa
        }

        // 2. Cambiar contraseña
        [HttpPut("change-password")] // Ruta para cambiar la contraseña
        [Authorize(Roles = "Customer,Admin")] // Requiere estar autenticado y tener el rol de Customer o Admin
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordDto changePasswordDto)
        {
            // Obtener el usuario actual usando el método GetCurrentUserAsync del repositorio
            var user = await _userRepository.GetCurrentUserAsync();

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." }); // Si el usuario no se encuentra, retorna 404
            }

            // Verificar que la contraseña actual sea correcta
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return Unauthorized(new { message = "Contraseña actual incorrecta." }); // Si la contraseña no coincide, retorna 401
            }

            // Cambiar la contraseña del usuario
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _userRepository.UpdateUserAsync(user); // Actualizar la contraseña en la base de datos

            return Ok(new { message = "Contraseña cambiada exitosamente." }); // Respuesta exitosa
        }

        // 3. Obtener todos los clientes
        [HttpGet("customers")] // Ruta para obtener los clientes
        [Authorize(Roles = "Admin")] // Solo accesible para usuarios con el rol de Admin
        public async Task<IActionResult> GetCustomers([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchQuery = "")
        {
            // Obtener los usuarios paginados desde el repositorio
            var paginatedUsers = await _userRepository.GetPaginatedUsersAsync(page, pageSize, searchQuery);

            // Convertir los usuarios obtenidos a un DTO de respuesta
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
                Items = userDtos, // Items con los usuarios
                TotalPages = paginatedUsers.TotalPages, // Total de páginas
                CurrentPage = paginatedUsers.CurrentPage // Página actual
            });
        }

        // 4. Actualizar el estado del usuario (habilitado/deshabilitado)
        [HttpPut("update-status")] // Ruta para actualizar el estado del usuario
        [Authorize(Roles = "Admin")] // Solo accesible para usuarios con el rol de Admin
        public async Task<IActionResult> UpdateUserStatus([FromForm] UpdateUserStatusDto updateUserStatusDto)
        {
            // Obtener el usuario por su ID
            var user = await _userRepository.GetUserByIdAsync(updateUserStatusDto.UserId);

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." }); // Si no se encuentra el usuario, retorna 404
            }

            // Actualizar el estado del usuario
            await _userRepository.UpdateUserStatusAsync(updateUserStatusDto.UserId, updateUserStatusDto.IsEnabled);
            var status = updateUserStatusDto.IsEnabled ? "habilitada" : "deshabilitada"; // Definir el estado del usuario

            return Ok(new { message = $"Cuenta de usuario {status} exitosamente." }); // Respuesta exitosa
        }
    }
}
