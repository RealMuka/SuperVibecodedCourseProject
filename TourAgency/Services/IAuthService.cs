using TourAgency.Models.Entities;

namespace TourAgency.Services;

public interface IAuthService
{
    Task<User> RegisterAsync(string username, string password);

    Task<User?> LoginAsync(string username, string password);

    Task<bool> UpdateDescriptionAsync(int userId, string newDescription);
}
