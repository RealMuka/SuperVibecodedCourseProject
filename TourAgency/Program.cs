using Microsoft.EntityFrameworkCore;
using TourAgency.Data;
using TourAgency.Models.DTOs;
using TourAgency.Services;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=touragency.db";

builder.Services.AddDbContext<TourAgencyDbContext>(options =>
    options.UseSqlite(connectionString));

// Services
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.UseCors("AllowAll");

// Serve static files (HTML, CSS, JS)
app.UseStaticFiles();
app.UseDefaultFiles();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TourAgencyDbContext>();
    await db.Database.EnsureCreatedAsync();
    await DbInitializer.InitializeAsync(db);
}

// ==================== TOUR ENDPOINTS ====================

app.MapGet("/api/tours", GetAllTours)
    .WithName("GetAllTours")
    .WithOpenApi()
    .Produces<List<TourSummaryDto>>();

app.MapGet("/api/tours/{id}", GetTourById)
    .WithName("GetTourById")
    .WithOpenApi()
    .Produces<TourDto>();

// ==================== REVIEW ENDPOINTS ====================

app.MapPost("/api/tours/{id}/reviews", AddReview)
    .WithName("AddReview")
    .WithOpenApi()
    .Accepts<CreateReviewDto>("application/json")
    .Produces<TourDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status400BadRequest);

app.MapGet("/api/categories", GetCategories)
    .WithName("GetCategories")
    .WithOpenApi()
    .Produces<List<string>>();

// ==================== USER ENDPOINTS ====================

app.MapGet("/api/users", GetAllUsers)
    .WithName("GetAllUsers")
    .WithOpenApi()
    .Produces<List<UserDto>>();

app.MapGet("/api/users/{id}", GetUserById)
    .WithName("GetUserById")
    .WithOpenApi()
    .Produces<UserDto>();

app.MapPost("/api/users", CreateUser)
    .WithName("CreateUser")
    .WithOpenApi()
    .Accepts<CreateUserRequest>("application/json")
    .Produces<UserDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest);

// ==================== HEALTH CHECK ====================

app.MapGet("/api/health", () => 
    Results.Ok(new { status = "ok", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();

// ==================== HANDLER METHODS ====================

async Task<IResult> GetAllTours(
    ITourService tourService,
    string? category,
    decimal? maxPrice,
    double? minRating)
{
    try
    {
        var tours = await tourService.GetAllToursAsync(category, maxPrice, minRating);
        return Results.Ok(tours);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
}

async Task<IResult> GetTourById(int id, ITourService tourService)
{
    try
    {
        var tour = await tourService.GetTourByIdAsync(id);
        return tour is null 
            ? Results.NotFound($"Tour {id} не найден") 
            : Results.Ok(tour);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
}

async Task<IResult> AddReview(int id, CreateReviewDto review, ITourService tourService)
{
    try
    {
        var tour = await tourService.AddReviewAsync(id, review);
        return Results.Ok(tour);
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
}

IResult GetCategories()
{
    var categories = new List<string> { "Все", "Море", "Горы", "Европа", "Азия" };
    return Results.Ok(categories);
}

async Task<IResult> GetAllUsers(IUserService userService)
{
    try
    {
        var users = await userService.GetAllUsersAsync();
        return Results.Ok(users);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
}

async Task<IResult> GetUserById(int id, IUserService userService)
{
    try
    {
        var user = await userService.GetUserByIdAsync(id);
        return user is null 
            ? Results.NotFound($"User {id} не найден") 
            : Results.Ok(user);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
}

async Task<IResult> CreateUser(CreateUserRequest request, IUserService userService)
{
    try
    {
        var user = await userService.CreateUserAsync(request.Name, request.Description);
        return Results.Ok(user);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
}

// ==================== REQUEST MODELS ====================

public record CreateUserRequest(string Name, string Description);
