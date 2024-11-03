using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TallerIDWMBackend.DTOs.User;
using TallerIDWMBackend.Interfaces;

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
        [Authorize]
        public async Task<IActionResult> EditProfile([FromBody] EditProfileDto editProfileDto)
        {
            var userId = GetUserIdFromClaims(); // Método para obtener el ID del usuario desde los claims
            var user = await _userRepository.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            user.Name = editProfileDto.Name;
            user.BirthDate = editProfileDto.BirthDate;
            user.Gender = editProfileDto.Gender;

            await _userRepository.UpdateUserAsync(user);

            return Ok(new { message = "Perfil actualizado exitosamente." });
        }

        // 2. Cambiar contraseña
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var userId = GetUserIdFromClaims();
            var user = await _userRepository.GetUserByIdAsync(userId);

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

        private long GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? long.Parse(userIdClaim.Value) : 0;
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

            return Ok(new PaginatedResponse<UserDto>
            {
                Items = userDtos,
                TotalPages = paginatedUsers.TotalPages,
                CurrentPage = paginatedUsers.CurrentPage
            });
        }

        [HttpPut("update-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusDto updateUserStatusDto)
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