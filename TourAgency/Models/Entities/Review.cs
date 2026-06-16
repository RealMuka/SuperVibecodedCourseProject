namespace TourAgency.Models.Entities;

public class Review
{
    public int Id { get; set; }
    
    public int TourId { get; set; }
    
    public string UserName { get; set; } = string.Empty;
    
    public string Text { get; set; } = string.Empty;
    
    public int Rating { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Tour Tour { get; set; } = null!;
}
