using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TallerIDWMBackend.Models;
using TallerIDWMBackend.Interfaces;
using BCrypt.Net;
using TallerIDWMBackend.DTOs.Auth;

namespace TallerIDWMBackend.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password);
        Task<string> RegisterAsync(RegisterDto registerDto);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Correo electrónico o contraseña incorrectos.");
            }

            if (!user.IsEnabled)
            {
                throw new UnauthorizedAccessException("La cuenta ingresada fue eliminada del sistema anteriormente.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 16)
            {
                throw new InvalidOperationException("Error en la configuración: La clave secreta de JWT no está configurada o es demasiado corta.");
            }

            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "Customer")
                    }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            if (await _userRepository.IsEmailOrRutRegisteredAsync(registerDto.Email, registerDto.Rut))
            {
                throw new InvalidOperationException("El correo electrónico o RUT ya están registrados.");
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

            return "Usuario registrado exitosamente.";
        }
    }
}
