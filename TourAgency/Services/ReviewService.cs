using Microsoft.EntityFrameworkCore;
using TourAgency.Data;
using TourAgency.Models.DTOs;

namespace TourAgency.Services;

public class ReviewService : IReviewService
{
    private readonly TourAgencyDbContext _db;
    
    public ReviewService(TourAgencyDbContext db)
    {
        _db = db;
    }
    
    public async Task<List<ReviewDto>> GetReviewsByTourIdAsync(int tourId)
    {
        return await _db.Reviews
            .Where(r => r.TourId == tourId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto(
                r.Id,
                r.UserName,
                r.Text,
                r.Rating,
                r.CreatedAt
            ))
            .ToListAsync();
    }
}
