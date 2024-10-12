using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TallerIDWMBackend.Models;
using TallerIDWMBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TallerIDWMBackend.DTOs;
using TallerIDWMBackend.DTOs.Auth;
namespace TallerIDWMBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _dataContext;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _configuration = configuration;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            if (await _dataContext.Users.AnyAsync(u => u.Email == registerDto.Email || u.Rut == registerDto.Rut))
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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            };

            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();

            return Ok("Usuario registrado exitosamente.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Verificar si el modelo es válido
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            // Buscar al usuario por su email
            var user = await _dataContext.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Correo electrónico o contraseña incorrectos.");
            }

            // Generar el token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

            // Verificar que la clave secreta esté configurada
            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 16)
            {
                return StatusCode(500, "Error en la configuración: La clave secreta de JWT no está configurada o es demasiado corta.");
            }

            // Convertir la clave secreta en bytes
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "Customer")
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Configurar expiración del token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Crear el token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Establecer la cookie con el token JWT
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // La cookie no será accesible vía JavaScript
                Secure = true, // Solo se enviará por HTTPS
                SameSite = SameSiteMode.Strict, // Evitar ataques CSRF
                Expires = token.ValidTo // Expirará al mismo tiempo que el token
            };

            // Añadir la cookie a la respuesta
            Response.Cookies.Append("jwt_token", tokenString, cookieOptions);

            // También puedes devolver la información en el cuerpo de la respuesta si es necesario
            return Ok(new
            {
                Message = "Inicio de sesión exitoso",
                Expiration = token.ValidTo
            });
        }
        
        [HttpPost("logout")]
        [Authorize]  // Asegúrate de que solo los usuarios autenticados puedan cerrar sesión
        public IActionResult Logout()
        {
            // Intentar obtener el token desde las cookies
            var token = HttpContext.Request.Cookies["jwt_token"];

            // Si no se encuentra el token en la cookie
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { message = "No se encontró el token en la cookie." });
            }

            // Opcionalmente: Aquí podrías añadir el token a una lista negra (Blacklist) si implementas una lógica de invalidación de tokens en tu aplicación

            // Eliminar la cookie jwt_token al hacer logout (establecer una expiración en el pasado)
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Asegura que la cookie se maneje a través de HTTPS
                Expires = DateTime.UtcNow.AddDays(-1), // Fecha en el pasado para eliminar la cookie
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("jwt_token", "", cookieOptions);

            return Ok(new { message = "Sesión cerrada correctamente." });
        }
    }
}