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


