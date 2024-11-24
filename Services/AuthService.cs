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
    /// <summary>
    /// Implementación del servicio de autenticación que gestiona el registro y el inicio de sesión de usuarios, 
    /// incluyendo la validación de credenciales y la generación de tokens JWT.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AuthService"/> con el repositorio de usuarios y la configuración de la aplicación.
        /// </summary>
        /// <param name="userRepository">Repositorio para interactuar con los usuarios en la base de datos.</param>
        /// <param name="configuration">Configuración de la aplicación que incluye valores como la clave secreta para JWT.</param>
        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }
        /// <summary>
        /// Inicia sesión de un usuario, validando las credenciales y generando un token JWT si son correctas.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario.</param>
        /// <param name="password">La contraseña del usuario.</param>
        /// <returns>Un token JWT que autoriza al usuario a acceder a los recursos protegidos.</returns>
        /// <exception cref="UnauthorizedAccessException">Se lanza si el correo electrónico o la contraseña son incorrectos, o si la cuenta está deshabilitada.</exception>
        /// <exception cref="InvalidOperationException">Se lanza si la clave secreta de JWT no está configurada o es demasiado corta.</exception>
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
        /// <summary>
        /// Registra un nuevo usuario en el sistema, validando que el correo electrónico y el RUT no estén ya registrados.
        /// </summary>
        /// <param name="registerDto">Los datos del usuario a registrar.</param>
        /// <returns>Un mensaje que indica el éxito del registro.</returns>
        /// <exception cref="InvalidOperationException">Se lanza si el correo electrónico o el RUT ya están registrados.</exception>
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
