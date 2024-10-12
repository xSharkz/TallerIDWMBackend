using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}