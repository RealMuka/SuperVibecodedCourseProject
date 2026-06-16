namespace TourAgency.Models.DTOs;

public record ReviewDto(
    int Id,
    string UserName,
    string Text,
    int Rating,
    DateTime CreatedAt
);

public record CreateReviewDto(
    string UserName,
    string Text,
    int Rating
);
