using Microsoft.EntityFrameworkCore;
using TourAgency.Models.Entities;

namespace TourAgency.Data;

public class TourAgencyDbContext : DbContext
{
    public TourAgencyDbContext(DbContextOptions<TourAgencyDbContext> options) 
        : base(options)
    {
    }
    
    public DbSet<Tour> Tours { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tour>(entity =>
        {
            entity.Property(t => t.Name).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Description).IsRequired();
            entity.Property(t => t.Price).IsRequired();
            entity.Property(t => t.Category).IsRequired().HasMaxLength(50);
        });
        
        modelBuilder.Entity<Review>(entity =>
        {
            entity.Property(r => r.UserName).IsRequired().HasMaxLength(100);
            entity.Property(r => r.Text).IsRequired().HasMaxLength(500);
            entity.Property(r => r.Rating).IsRequired();
            entity.HasIndex(r => r.TourId);
        });
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();
        });
        
        base.OnModelCreating(modelBuilder);
    }
}
