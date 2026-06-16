using TourAgency.Models.Entities;

namespace TourAgency.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(TourAgencyDbContext db)
    {
        if (db.Tours.Any()) 
            return;
        
        var tours = new List<Tour>
        {
            new Tour
            {
                Name = "Мальдивы: Пассивный Релакс",
                Description = "10 дней на премиальном resort с private beach, infinity pool и ежедневным спа. Идеально для тех кто хочет просто лежать и наслаждаться.",
                Price = 185000,
                ImageUrl = "https://images.unsplash.com/photo-1590523721851-413d5c4ee7f8?w=800",
                Category = "Море",
                AverageRating = 4.8,
                Reviews =
                [
                    new Review { UserName = "Анна", Text = "Лучший тур в жизни! Всё было идеально.", Rating = 5, CreatedAt = DateTime.UtcNow.AddDays(-10) },
                    new Review { UserName = "Дмитрий", Text = "Отлично, но дорого.", Rating = 4, CreatedAt = DateTime.UtcNow.AddDays(-5) }
                ]
            },
            new Tour
            {
                Name = "Алтай: Горный Экстрим",
                Description = "7 дней активного трекинга, верхолазание, horse riding и ночёвки в eco-camps. Для тех кто любит движение.",
                Price = 92000,
                ImageUrl = "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?w=800",
                Category = "Горы",
                AverageRating = 4.6,
                Reviews =
                [
                    new Review { UserName = "Сергей", Text = "Очень круто! Буду ещё раз.", Rating = 5, CreatedAt = DateTime.UtcNow.AddDays(-7) }
                ]
            },
            new Tour
            {
                Name = "Европа: Классика за 15 дней",
                Description = "Париж, Рим, Венеция, Прага, Барселона. Все достопримечательности, лучшие рестораны, шопинг.",
                Price = 245000,
                ImageUrl = "https://images.unsplash.com/photo-1499856871940-a09627c69f23?w=800",
                Category = "Европа",
                AverageRating = 4.7,
                Reviews = []
            },
            new Tour
            {
                Name = "Вьетнам: Тайные Ландшафты",
                Description = "12 дней в джунглях, пляжах и горных villages. Local food, street meals, authentic experiences.",
                Price = 78000,
                ImageUrl = "https://images.unsplash.com/photo-1528127220168-9c6042b42a4a?w=800",
                Category = "Азия",
                AverageRating = 4.5,
                Reviews = []
            },
            new Tour
            {
                Name = "Таиланд: Пхукет Premium",
                Description = "14 дней в 5* resort с private beach club, daily spa, infinity pools и все включено.",
                Price = 156000,
                ImageUrl = "https://images.unsplash.com/photo-1589394885656-17704898cd56?w=800",
                Category = "Море",
                AverageRating = 4.9,
                Reviews = []
            },
            new Tour
            {
                Name = "Норвегия: Фёрды и Верхолазание",
                Description = "10 дней в самых красивых фёрдах Норвегии. Трекинг, climbing, boat tours.",
                Price = 215000,
                ImageUrl = "https://images.unsplash.com/photo-1507025875561-8c931e60c96c?w=800",
                Category = "Горы",
                AverageRating = 4.8,
                Reviews = []
            },
            new Tour
            {
                Name = "Испания: Барселона + Мадрид",
                Description = "8 дней в самых крутых городах Испании. Art, food, nightlife, shopping.",
                Price = 134000,
                ImageUrl = "https://images.unsplash.com/photo-1543783207-ec64e4d95325?w=800",
                Category = "Европа",
                AverageRating = 4.6,
                Reviews = []
            },
            new Tour
            {
                Name = "Япония: Токио + Киото",
                Description = "12 дней в Японии. Temples, ramen, sushi, anime, tech. Полный immersion.",
                Price = 298000,
                ImageUrl = "https://images.unsplash.com/photo-1493976040374-85c8e12f0c0e?w=800",
                Category = "Азия",
                AverageRating = 4.9,
                Reviews = []
            }
        };
        
        db.Tours.AddRange(tours);
        
        var users = new List<User>
        {
            new User
            {
                Name = "Елена Волкова",
                Description = "Люблю путешествия и новые места. Влюблена в моря и пляжи.",
                ToursRequested = 12,
                Reviews = []
            },
            new User
            {
                Name = "Максим Котов",
                Description = "Активный турист. Люблю горы, трекинг и экстрим.",
                ToursRequested = 8,
                Reviews = []
            },
            new User
            {
                Name = "Ольга Смирнова",
                Description = "Предпочитаю комфорт и релакс. Европа — моя любовь.",
                ToursRequested = 15,
                Reviews = []
            }
        };
        
        db.Users.AddRange(users);
        
        await db.SaveChangesAsync();
    }
}
