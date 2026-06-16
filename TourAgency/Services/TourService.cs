using Microsoft.EntityFrameworkCore;
using TourAgency.Data;
using TourAgency.Models.DTOs;
using TourAgency.Models.Entities;

namespace TourAgency.Services;

public class TourService : ITourService
{
    private readonly TourAgencyDbContext _db;
    
    public TourService(TourAgencyDbContext db)
    {
        _db = db;
    }
    
    public async Task<List<TourSummaryDto>> GetAllToursAsync(
        string? category, 
        decimal? maxPrice, 
        double? minRating)
    {
        var query = _db.Tours
            .Include(t => t.Reviews)
            .AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(category) && category != "Все")
        {
            query = query.Where(t => t.Category == category);
        }
        
        if (maxPrice.HasValue)
        {
            query = query.Where(t => t.Price <= maxPrice);
        }
        
        if (minRating.HasValue)
        {
            query = query.Where(t => t.AverageRating >= minRating);
        }
        
        return await query
            .OrderByDescending(t => t.AverageRating)
            .Select(t => new TourSummaryDto(
                t.Id,
                t.Name,
                t.Description.Length > 150 
                    ? t.Description[..150] + "..." 
                    : t.Description,
                t.Price,
                t.ImageUrl,
                t.Category,
                t.AverageRating
            ))
            .ToListAsync();
    }
    
    public async Task<TourDto?> GetTourByIdAsync(int id)
    {
        var tour = await _db.Tours
            .Include(t => t.Reviews)
            .FirstOrDefaultAsync(t => t.Id == id);
        
        if (tour == null) 
            return null;
        
        return new TourDto(
            tour.Id,
            tour.Name,
            tour.Description,
            tour.Price,
            tour.ImageUrl,
            tour.Category,
            tour.AverageRating,
            tour.Reviews
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
    
    public async Task<TourDto> AddReviewAsync(int tourId, CreateReviewDto review)
    {
        var tour = await _db.Tours
            .Include(t => t.Reviews)
            .FirstOrDefaultAsync(t => t.Id == tourId);
        
        if (tour == null)
            throw new KeyNotFoundException($"Tour {tourId} не найден");
        
        ValidateReview(review);
        
        var newReview = new Review
        {
            TourId = tourId,
            UserName = review.UserName.Trim(),
            Text = review.Text.Trim(),
            Rating = review.Rating,
            CreatedAt = DateTime.UtcNow
        };
        
        tour.Reviews.Add(newReview);
        tour.AverageRating = Math.Round(tour.Reviews.Average(r => r.Rating), 2);
        
        await _db.SaveChangesAsync();
        
        return (await GetTourByIdAsync(tourId))!;
    }
    
    private static void ValidateReview(CreateReviewDto review)
    {
        if (review.Rating is < 1 or > 5)
            throw new ArgumentException("Рейтинг должен быть от 1 до 5");
        
        if (string.IsNullOrWhiteSpace(review.UserName) || review.UserName.Length > 100)
            throw new ArgumentException("Имя пользователя некорректно");
        
        if (string.IsNullOrWhiteSpace(review.Text) || review.Text.Length > 500)
            throw new ArgumentException("Отзыв некорректен");
    }
}
