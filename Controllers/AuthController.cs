using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TallerIDWMBackend.DTOs;
using TallerIDWMBackend.DTOs.Auth;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.Services;

namespace TallerIDWMBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;  // Agregar dependencia

        public AuthController(IAuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;  // Asignar el repositorio
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var message = await _authService.RegisterAsync(registerDto);
                return Ok(message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var token = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(24)
                };

                Response.Cookies.Append("jwt_token", token, cookieOptions);

                return Ok(new { Message = "Inicio de sesión exitoso", Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(-1),
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("jwt_token", "", cookieOptions);

            return Ok(new { message = "Sesión cerrada correctamente." });
        }

        [HttpDelete("delete-account")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            // Obtener el ID del usuario desde el contexto
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return BadRequest("No se pudo encontrar la información del usuario.");
            }

            long userId = long.Parse(userIdClaim.Value);

            // Obtener el usuario a eliminar
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Deshabilitar el usuario
            user.IsEnabled = false;
            await _userRepository.UpdateUserAsync(user);

            Logout();
            return Ok("Cuenta eliminada exitosamente.");
        }
    }
}
