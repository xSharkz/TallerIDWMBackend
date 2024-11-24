using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TallerIDWMBackend.DTOs.Auth;

namespace TallerIDWMBackend.Interfaces
{
    /// <summary>
    /// Servicio de autenticación que maneja el registro y el inicio de sesión de los usuarios.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Inicia sesión de un usuario, generando un token JWT si las credenciales son válidas.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario.</param>
        /// <param name="password">La contraseña del usuario.</param>
        /// <returns>Un token JWT que autoriza al usuario a acceder a los recursos protegidos.</returns>
        Task<string> LoginAsync(string email, string password);
        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="registerDto">Datos del usuario a registrar.</param>
        /// <returns>Un mensaje de confirmación sobre el registro del usuario.</returns>
        Task<string> RegisterAsync(RegisterDto registerDto);
    }
}