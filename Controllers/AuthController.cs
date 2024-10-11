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
        public async Task<IActionResult> Register([FromBody] Register register)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (await _dataContext.Users.AnyAsync(u => u.Email == register.Email || u.Rut == register.Rut))
            {
                return BadRequest("El correo electrónico o RUT ya están registrados.");
            }

            var user = new User
            {
                Rut = register.Rut,
                Name = register.Name,
                BirthDate = register.BirthDate,
                Email = register.Email,
                Gender = register.Gender,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(register.Password)
            };

            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();

            return Ok("Usuario registrado exitosamente.");
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            // Verificar si el modelo es válido
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            // Buscar al usuario por su email
            var user = await _dataContext.Users.SingleOrDefaultAsync(u => u.Email == login.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
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

            // Devolver el token y su expiración
            return Ok(new
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = token.ValidTo
            });
        }
        
        [HttpPost("logout")]
        [Authorize]  // Asegúrate de que solo los usuarios autenticados puedan cerrar sesión
        public IActionResult Logout()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { message = "Token no proporcionado." });
            }

            return Ok(new { message = "Sesión cerrada correctamente." });
        }
    }
}