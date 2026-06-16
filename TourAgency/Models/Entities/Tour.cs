namespace TourAgency.Models.Entities;

public class Tour
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    public string ImageUrl { get; set; } = string.Empty;
    
    public string Category { get; set; } = string.Empty;
    
    public double AverageRating { get; set; }
    
    public List<Review> Reviews { get; set; } = [];
}
