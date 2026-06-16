using TourAgency.Models.DTOs;

namespace TourAgency.Services;

public interface IReviewService
{
    Task<List<ReviewDto>> GetReviewsByTourIdAsync(int tourId);
}
