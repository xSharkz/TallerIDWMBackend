using Microsoft.EntityFrameworkCore;
using TallerIDWMBackend.Data;
using TallerIDWMBackend.DTOs.User;
using TallerIDWMBackend.Interfaces;
using TallerIDWMBackend.Models;
using TallerIDWMBackend.Services;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dataContext;
    private readonly UserContextService _userContextService;

    public UserRepository(ApplicationDbContext dataContext, UserContextService userContextService)
    {
        _dataContext = dataContext;
        _userContextService = userContextService;
    }

    public async Task<User> GetCurrentUserAsync()
    {
        long userId = _userContextService.GetUserIdFromClaims();
        return await GetUserByIdAsync(userId);
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
        await _dataContext.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dataContext.SaveChangesAsync();
    }

    public async Task<PaginatedResponseDto<User>> GetPaginatedUsersAsync(int page, int pageSize, string searchQuery)
    {
        var query = _dataContext.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(u => u.Name.Contains(searchQuery) || u.Email.Contains(searchQuery));
        }

        int totalUsers = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponseDto<User>
        {
            Items = users,
            TotalPages = totalPages,
            CurrentPage = page
        };
    }

    public async Task UpdateUserStatusAsync(long userId, bool isEnabled)
    {
        var user = await _dataContext.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsEnabled = isEnabled;
            _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();
        }
    }
}
