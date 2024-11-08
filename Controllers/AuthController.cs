using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.Models;
using TallerIDWMBackend.DTOs.Auth;
using TallerIDWMBackend.DTOs;

namespace TallerIDWMBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userRepository.IsEmailOrRutRegisteredAsync(registerDto.Email, registerDto.Rut))
            {
                return BadRequest("El correo electrónico o RUT ya están registrados.");
            }

            var user = new User
            {
                Rut = registerDto.Rut,
                Name = registerDto.Name,
                BirthDate = registerDto.BirthDate,
                Email = registerDto.Email,
                Gender = registerDto.Gender,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
            };

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return Ok("Usuario registrado exitosamente.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Correo electrónico o contraseña incorrectos.");
            }
            if (user.IsEnabled == false){
                return Unauthorized("La cuenta ingresada fue eliminada del sistema anteriormente.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 16)
            {
                return StatusCode(500, "Error en la configuración: La clave secreta de JWT no está configurada o es demasiado corta.");
            }

            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "Customer")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = token.ValidTo
            };

            Response.Cookies.Append("jwt_token", tokenString, cookieOptions);

            return Ok(new { Message = "Inicio de sesión exitoso", Expiration = token.ValidTo });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var token = HttpContext.Request.Cookies["jwt_token"];
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { message = "No se encontró el token en la cookie." });
            }

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