using Microsoft.EntityFrameworkCore;
using TourAgency.Data;
using TourAgency.Models.DTOs;
using TourAgency.Models.Entities;

namespace TourAgency.Services;

public class UserService : IUserService
{
    private readonly TourAgencyDbContext _db;
    
    public UserService(TourAgencyDbContext db)
    {
        _db = db;
    }
    
    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _db.Users
            .Include(u => u.Reviews)
            .FirstOrDefaultAsync(u => u.Id == id);
        
        if (user == null) 
            return null;
        
        return MapToDto(user);
    }
    
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _db.Users
            .Include(u => u.Reviews)
            .Select(u => MapToDto(u))
            .ToListAsync();
    }
    
    public async Task<UserDto> CreateUserAsync(string name, string description)
    {
        ValidateUser(name, description);
        
        var user = new User
        {
            Name = name.Trim(),
            Description = description.Trim(),
            ToursRequested = 0
        };
        
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        
        return MapToDto(user);
    }
    
    private static UserDto MapToDto(User user)
    {
        return new UserDto(
            user.Id,
            user.Name,
            user.Description,
            user.ToursRequested,
            user.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto(
                    r.Id,
                    r.UserName,
                    r.Text,
                    r.Rating,
                    r.CreatedAt
                ))
                .ToList()
        );
    }
    
    private static void ValidateUser(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
            throw new ArgumentException("Имя некорректно");
        
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Описание некорректно");
    }
}
