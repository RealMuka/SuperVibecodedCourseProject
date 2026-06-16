using TourAgency.Models.DTOs;

namespace TourAgency.Services;

public interface ITourService
{
    Task<List<TourSummaryDto>> GetAllToursAsync(string? category, decimal? maxPrice, double? minRating);
    Task<TourDto?> GetTourByIdAsync(int id);
    Task<TourDto> AddReviewAsync(int tourId, CreateReviewDto review);
}
