namespace TourAgency.Models.Entities;

public class User
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;
    
    public int ToursRequested { get; set; } = 0;
    
    public List<Review> Reviews { get; set; } = [];
}
