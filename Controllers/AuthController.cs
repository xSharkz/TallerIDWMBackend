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
        private readonly IAuthService _authService; // Servicio para manejar operaciones de autenticación
        private readonly IUserRepository _userRepository;  // Repositorio para manejar la información del usuario

        /// <summary>
        /// Constructor que inyecta las dependencias necesarias.
        /// </summary>
        /// <param name="authService">Servicio de autenticación.</param>
        /// <param name="userRepository">Repositorio de usuarios.</param>
        public AuthController(IAuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;  // Asignar el repositorio
        }

        /// <summary>
        /// Registra un nuevo usuario.
        /// </summary>
        /// <param name="registerDto">Datos del nuevo usuario.</param>
        /// <returns>Un mensaje con el resultado del registro.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // Si el modelo no es válido, devuelve BadRequest

            try
            {
                var message = await _authService.RegisterAsync(registerDto);
                return Ok(message);  // Devuelve mensaje de éxito
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);  // Si ocurre un error, devuelve BadRequest con el mensaje de error
            }
        }

        /// <summary>
        /// Inicia sesión de un usuario existente.
        /// </summary>
        /// <param name="loginDto">Datos de inicio de sesión (correo y contraseña).</param>
        /// <returns>Un mensaje con el resultado del inicio de sesión y un token JWT.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // Si el modelo no es válido, devuelve BadRequest

            try
            {
                var token = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

                // Configura las cookies con el token JWT
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
                return Unauthorized(ex.Message);  // Si las credenciales son incorrectas, devuelve Unauthorized
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);  // Si ocurre un error interno, devuelve un error 500
            }
        }

        /// <summary>
        /// Cierra la sesión del usuario actual.
        /// </summary>
        /// <returns>Un mensaje indicando que la sesión fue cerrada correctamente.</returns>
        [HttpPost("logout")]
        [Authorize]  // Requiere que el usuario esté autenticado
        public IActionResult Logout()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(-1),  // Establece la expiración en el pasado para eliminarla
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("jwt_token", "", cookieOptions);
            return Ok(new { message = "Sesión cerrada correctamente." });
        }

        /// <summary>
        /// Elimina la cuenta del usuario deshabilitándola.
        /// </summary>
        /// <returns>Un mensaje con el resultado de la eliminación de la cuenta.</returns>
        [HttpDelete("delete-account")]
        [Authorize]  // Requiere que el usuario esté autenticado
        public async Task<IActionResult> DeleteAccount()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return BadRequest("No se pudo encontrar la información del usuario.");
            }

            long userId = long.Parse(userIdClaim.Value);
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Deshabilita la cuenta del usuario
            user.IsEnabled = false;
            await _userRepository.UpdateUserAsync(user);

            // Cierra sesión después de la eliminación de la cuenta
            Logout();

            return Ok("Cuenta eliminada exitosamente.");
        }
    }
}
