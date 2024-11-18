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

        // Constructor que inyecta las dependencias necesarias
        public AuthController(IAuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;  // Asignar el repositorio
        }

        // POST: api/auth/register
        // Método para registrar un nuevo usuario
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            // Verifica que el modelo de datos sea válido
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // Si no es válido, devuelve BadRequest con los detalles del error

            try
            {
                // Llama al servicio de autenticación para registrar al usuario
                var message = await _authService.RegisterAsync(registerDto);
                return Ok(message);  // Si el registro es exitoso, devuelve un mensaje de éxito
            }
            catch (InvalidOperationException ex)
            {
                // Si ocurre un error en el proceso, devuelve BadRequest con el mensaje de error
                return BadRequest(ex.Message);
            }
        }

        // POST: api/auth/login
        // Método para iniciar sesión con un usuario existente
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            // Verifica que el modelo de datos sea válido
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // Si no es válido, devuelve BadRequest con los detalles del error

            try
            {
                // Llama al servicio de autenticación para obtener un token JWT
                var token = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

                // Configuración de las cookies para almacenar el token JWT
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(24)
                };

                // Guarda el token en una cookie con las opciones de seguridad configuradas
                Response.Cookies.Append("jwt_token", token, cookieOptions);

                // Devuelve un mensaje de éxito junto con el token
                return Ok(new { Message = "Inicio de sesión exitoso", Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Si las credenciales son incorrectas, devuelve Unauthorized con el mensaje de error
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Si ocurre un error interno en el proceso, devuelve un error 500
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/auth/logout
        // Método para cerrar sesión del usuario actual
        [HttpPost("logout")]
        [Authorize]  // Requiere que el usuario esté autenticado
        public IActionResult Logout()
        {
            // Configuración de las cookies para eliminar el token JWT
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(-1),  // Establece la expiración en el pasado para eliminarla
                SameSite = SameSiteMode.Strict
            };

            // Elimina el token JWT de las cookies
            Response.Cookies.Append("jwt_token", "", cookieOptions);

            // Devuelve un mensaje de éxito indicando que la sesión fue cerrada correctamente
            return Ok(new { message = "Sesión cerrada correctamente." });
        }

        // DELETE: api/auth/delete-account
        // Método para eliminar la cuenta del usuario deshabilitándola
        [HttpDelete("delete-account")]
        [Authorize]  // Requiere que el usuario esté autenticado
        public async Task<IActionResult> DeleteAccount()
        {
            // Obtiene el ID del usuario desde el contexto de la sesión
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return BadRequest("No se pudo encontrar la información del usuario.");  // Si no se encuentra el ID del usuario, devuelve error
            }

            long userId = long.Parse(userIdClaim.Value);

            // Obtiene el usuario a eliminar
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");  // Si el usuario no existe, devuelve error
            }

            // Deshabilita la cuenta del usuario (no la elimina permanentemente)
            user.IsEnabled = false;
            await _userRepository.UpdateUserAsync(user);

            // Cierra sesión del usuario después de la eliminación de la cuenta
            Logout();

            // Devuelve un mensaje de éxito
            return Ok("Cuenta eliminada exitosamente.");
        }
    }
}
