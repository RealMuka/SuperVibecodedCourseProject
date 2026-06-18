using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TourAgency.Data;
using TourAgency.Models.Entities;

namespace TourAgency.Services;

public class AuthService(TourAgencyDbContext context) : IAuthService
{
    private TourAgencyDbContext _context = context;

    private const int SaltSize = 16;          
    private const int HashSize = 32;          
    private const int Iterations = 3;        
    private const int MemorySize = 65536;    
    private const int Parallelism = 4;       

    public async Task<User> RegisterAsync(string username, string password)
    {
        var existingUser = await _context.Set<User>().FirstOrDefaultAsync(u => u.Name == username);
        if (existingUser != null)
        {
            throw new Exception("Пользователь с таким именем уже существует.");
        }

        string hashedPassword = HashPassword(password);

        var newUser = new User
        {
            Name = username,
            PasswordHash = hashedPassword,
            Description = string.Empty,
            ToursRequested = 0,
            Reviews = new List<Review>()
        };

        await _context.Set<User>().AddAsync(newUser);
        await _context.SaveChangesAsync();

        return newUser;
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.Name == username);
        if (user == null)
        {
            return null; 
        }

        bool isPasswordValid = VerifyPassword(password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return null;
        }

        return user;
    }

    public async Task<bool> UpdateDescriptionAsync(int userId, string newDescription)
    {
        var user = await _context.Set<User>().FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.Description = newDescription;
        await _context.SaveChangesAsync();
        return true;
    }

    #region Приватные хелперы для работы с Argon2id

    private string HashPassword(string password)
    {
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = Parallelism,
            MemorySize = MemorySize,
            Iterations = Iterations
        };

        byte[] hash = argon2.GetBytes(HashSize);

        string saltBase64 = Convert.ToBase64String(salt);
        string hashBase64 = Convert.ToBase64String(hash);

        return $"$argon2id$v=19$m={MemorySize},t={Iterations},p={Parallelism}${saltBase64}${hashBase64}";
    }

    private bool VerifyPassword(string password, string storedHashFormat)
    {
        try
        {
            var parts = storedHashFormat.Split('$');
            if (parts.Length < 6) return false;

            var paramsPart = parts[3].Split(',');
            int memory = int.Parse(paramsPart[0].Replace("m=", ""));
            int iterations = int.Parse(paramsPart[1].Replace("t=", ""));
            int parallelism = int.Parse(paramsPart[2].Replace("p=", ""));

            byte[] salt = Convert.FromBase64String(parts[4]);
            byte[] expectedHash = Convert.FromBase64String(parts[5]);

            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = parallelism,
                MemorySize = memory,
                Iterations = iterations
            };

            byte[] actualHash = argon2.GetBytes(expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
        }
        catch
        {
            return false;
        }
    }

    #endregion
}
