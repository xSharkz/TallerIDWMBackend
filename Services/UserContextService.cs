using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TallerIDWMBackend.Services
{
    /// <summary>
    /// Servicio que proporciona acceso al contexto del usuario en la aplicación,
    /// permitiendo obtener información del usuario autenticado a partir de los claims.
    /// </summary>
    public class UserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UserContextService"/>.
        /// </summary>
        /// <param name="httpContextAccessor">Accesor para acceder al contexto HTTP actual.</param>
        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        /// <summary>
        /// Obtiene el ID del usuario autenticado a partir de los claims del token JWT.
        /// </summary>
        /// <returns>El ID del usuario o 0 si no se encuentra el claim correspondiente.</returns>
        public long GetUserIdFromClaims()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? long.Parse(userIdClaim.Value) : 0;
        }
    }
}
