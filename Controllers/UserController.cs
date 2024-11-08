using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TallerIDWMBackend.DTOs.User;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.Repository;

namespace TallerIDWMBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // 1. Editar perfil
        [HttpPut("edit-profile")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> EditProfile([FromForm] EditProfileDto editProfileDto)
        {
            // Obtener el usuario actual usando el método GetCurrentUserAsync del repositorio
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

        // 2. Cambiar contraseña
        [HttpPut("change-password")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordDto changePasswordDto)
        {
            // Obtener el usuario actual usando el método GetCurrentUserAsync del repositorio
            var user = await _userRepository.GetCurrentUserAsync();

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            // Verificar la contraseña actual
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return Unauthorized(new { message = "Contraseña actual incorrecta." });
            }

            // Cambiar la contraseña
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _userRepository.UpdateUserAsync(user);

            return Ok(new { message = "Contraseña cambiada exitosamente." });
        }

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