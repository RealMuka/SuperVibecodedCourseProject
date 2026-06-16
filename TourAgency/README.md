# Tour Agency Backend

ASP.NET Minimal API для сайта турагенства на **.NET 10** с современными C# 14 features.

## Технологии

- **.NET 10** ✨ Latest LTS
- **ASP.NET Minimal API**
- **Entity Framework Core 10**
- **SQLite**
- **C# 14+**

## Запуск

```bash
# Восстановить пакеты
dotnet restore

# Запустить приложение
dotnet run
```

Приложение будет доступно по адресу: `http://localhost:5000`

## API Endpoints

### Tours
- `GET /api/tours` - Получить все туры (с фильтром)
- `GET /api/tours/{id}` - Получить тур по ID
- `POST /api/tours/{id}/reviews` - Добавить отзыв
- `GET /api/categories` - Получить категории

### Users
- `GET /api/users` - Получить всех пользователей
- `GET /api/users/{id}` - Получить пользователя по ID
- `POST /api/users` - Создать пользователя

### Health
- `GET /api/health` - Health check

## Структура проекта

```
TourAgency/
├── Models/
│   ├── Entities/          # Entity классы
│   │   ├── Tour.cs
│   │   ├── Review.cs
│   │   └── User.cs
│   └── DTOs/              # Data Transfer Objects
│       ├── TourDto.cs
│       ├── ReviewDto.cs
│       └── UserDto.cs
├── Data/
│   ├── TourAgencyDbContext.cs
│   └── DbInitializer.cs
├── Services/              # Business logic
│   ├── ITourService.cs
│   ├── TourService.cs
│   ├── IReviewService.cs
│   ├── ReviewService.cs
│   ├── IUserService.cs
│   └── UserService.cs
├── Properties/
│   └── launchSettings.json
├── appsettings.json
├── appsettings.Development.json
├── Program.cs             # Minimal API endpoints
├── TourAgency.csproj
└── README.md
```

## Примеры API запросов

### cURL

```bash
# Получить все туры
curl "http://localhost:5000/api/tours"

# Получить туры по категории
curl "http://localhost:5000/api/tours?category=Море"

# Получить туры с фильтром по цене
curl "http://localhost:5000/api/tours?maxPrice=150000"

# Получить конкретный тур
curl "http://localhost:5000/api/tours/1"

# Добавить отзыв
curl -X POST "http://localhost:5000/api/tours/1/reviews" \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "Иван",
    "text": "Отличный тур!",
    "rating": 5
  }'

# Получить пользователя
curl "http://localhost:5000/api/users/1"

# Создать пользователя
curl -X POST "http://localhost:5000/api/users" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Иван Петров",
    "description": "Люблю путешествия"
  }'
```

### JavaScript Fetch

```javascript
// Получить все туры
const tours = await fetch('http://localhost:5000/api/tours').then(r => r.json());

// Фильтр по категории
const filtered = await fetch(
  'http://localhost:5000/api/tours?category=Море&maxPrice=200000'
).then(r => r.json());

// Добавить отзыв
const review = await fetch('http://localhost:5000/api/tours/1/reviews', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    userName: 'Анна',
    text: 'Превосходно!',
    rating: 5
  })
}).then(r => r.json());
```

## Особенности .NET 10

✅ **Primary Constructors** - используются в сервисах
✅ **Collection Expressions** - `[]` для инициализации списков
✅ **Latest EF Core 10** - лучшая производительность
✅ **C# 14 Features** - современный синтаксис
✅ **Async/Await везде** - полная асинхронность
✅ **Dependency Injection** - встроенное в ASP.NET
✅ **Minimal API** - чистый и простой код

## Data

- 8 туров с категориями (Море, Горы, Европа, Азия)
- 3 пользователя
- Система отзывов
- SQLite база данных

Проект production-ready и легко расширяется! 🚀
