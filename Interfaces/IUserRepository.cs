using TallerIDWMBackend.DTOs.User;
using TallerIDWMBackend.Models;

namespace TallerIDWMBackend.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(long id);
        Task UpdateUserAsync(User user);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> IsEmailOrRutRegisteredAsync(string email, string rut);
        Task AddUserAsync(User user);
        Task SaveChangesAsync();
        Task<PaginatedResponse<User>> GetPaginatedUsersAsync(int page, int pageSize, string searchQuery);
        Task UpdateUserStatusAsync(long userId, bool isEnabled);
    }
}
