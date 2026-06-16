namespace TourAgency.Models.DTOs;

public record TourDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    string Category,
    double AverageRating,
    List<ReviewDto> Reviews
);

public record TourSummaryDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    string Category,
    double AverageRating
);
