using Microsoft.EntityFrameworkCore;
using TourAgency.Data;
using TourAgency.Models.DTOs;

namespace TourAgency.Services;

public class ReviewService(TourAgencyDbContext db) : IReviewService
{
    public async Task<List<ReviewDto>> GetReviewsByTourIdAsync(int tourId)
    {
        return await db.Reviews
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
