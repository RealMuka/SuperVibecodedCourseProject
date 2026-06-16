namespace TourAgency.Models.DTOs;

public record UserDto(
    int Id,
    string Name,
    string Description,
    int ToursRequested,
    List<ReviewDto> Reviews
);
