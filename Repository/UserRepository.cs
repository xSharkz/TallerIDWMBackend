using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TallerIDWMBackend.Data;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.Models;

namespace TallerIDWMBackend.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dataContext;

        public UserRepository(ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<User> GetUserByIdAsync(long id)
        {
            return await _dataContext.Users.FindAsync(id);
        }

        public async Task UpdateUserAsync(User user)
        {
            _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _dataContext.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> IsEmailOrRutRegisteredAsync(string email, string rut)
        {
            return await _dataContext.Users.AnyAsync(u => u.Email == email || u.Rut == rut);
        }

        public async Task AddUserAsync(User user)
        {
            _dataContext.Users.Add(user);
        }

        public async Task SaveChangesAsync()
        {
            await _dataContext.SaveChangesAsync();
        }   
    }
}