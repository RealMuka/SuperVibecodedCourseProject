using TourAgency.Models.DTOs;

namespace TourAgency.Services;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto> CreateUserAsync(string name, string description);
}
