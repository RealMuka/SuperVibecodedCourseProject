using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using TourAgency.Data;
using TourAgency.Models.DTOs;
using TourAgency.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.LoginPath = "/auth";
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=touragency.db";

builder.Services.AddDbContext<TourAgencyDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.Split('?')[0].TrimEnd('/').ToLowerInvariant();

    if (path == "/auth.html")
    {
        context.Response.Redirect("/auth");
        return;
    }

    if (path == "/index.html")
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Response.Redirect("/auth");
            return;
        }

        context.Response.Redirect("/tours");
        return;
    }

    if (path == "/profile.html")
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Response.Redirect("/auth");
            return;
        }

        context.Response.Redirect("/profile");
        return;
    }

    if (path == "/tour.html")
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Response.Redirect("/auth");
            return;
        }

        var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;
        context.Response.Redirect($"/tour{queryString}");
        return;
    }

    await next();
});

app.UseStaticFiles();
app.UseDefaultFiles();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TourAgencyDbContext>();
    await db.Database.EnsureCreatedAsync();
    await DbInitializer.InitializeAsync(db);
}

var webRoot = app.Environment.WebRootPath;

app.MapGet("/", () => Results.Redirect("/tours"))
    .RequireAuthorization();

app.MapGet("/auth", () => Results.File(Path.Combine(webRoot, "auth.html"), "text/html"));

app.MapGet("/tours", () => Results.File(Path.Combine(webRoot, "index.html"), "text/html"))
    .RequireAuthorization();

app.MapGet("/profile", () => Results.File(Path.Combine(webRoot, "profile.html"), "text/html"))
    .RequireAuthorization();

app.MapGet("/tour", () => Results.File(Path.Combine(webRoot, "tour.html"), "text/html"))
    .RequireAuthorization();

app.MapGet("/tour/{id:int}", (int id) => Results.Redirect($"/tour?id={id}"))
    .RequireAuthorization();

app.MapGet("/authorization", () => Results.Redirect("/auth"));

app.MapGet("/api/tours", GetAllTours)
    .Produces<List<TourSummaryDto>>();

app.MapGet("/api/tours/{id}", GetTourById)
    .Produces<TourDto>();

app.MapPost("/api/tours/{id}/reviews", AddReview)
    .Accepts<CreateReviewDto>("application/json")
    .Produces<TourDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status400BadRequest);

app.MapGet("/api/categories", GetCategories)
    .Produces<List<string>>();


app.MapGet("/api/users", GetAllUsers)
    .Produces<List<UserDto>>();

app.MapGet("/api/users/{id}", GetUserById)
    .Produces<UserDto>();

app.MapPost("/api/auth/login", LoginUser)
    .Accepts<AuthRequest>("application/json")
    .Produces<UserDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest);

app.MapPost("/api/auth/register", RegisterUser)
    .Accepts<AuthRequest>("application/json")
    .Produces<UserDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest);

app.MapGet("/api/auth/me", GetCurrentUser)
    .Produces<UserDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);

app.MapPost("/api/auth/logout", LogoutUser);


app.MapGet("/api/health", () => 
    Results.Ok(new { status = "ok", timestamp = DateTime.UtcNow }));

app.Run();

async Task<IResult> LoginUser(AuthRequest request, IAuthService service, HttpContext httpContext)
{
    var user = await service.LoginAsync(request.name, request.password);
    if (user == null)
    {
        return Results.Json(new { message = "Неверное имя пользователя или пароль" }, statusCode: 400);
    }
    
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name)
    };
    
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);
    
    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    
    return Results.Ok(new 
    {
        userId = user.Id,
        userName = user.Name
    });
}

async Task<IResult> RegisterUser(AuthRequest request, IAuthService service)
{
    try
    {
        var user = await service.RegisterAsync(request.name, request.password);
        return Results.Ok(new 
        {
            userId = user.Id,
            userName = user.Name
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
}

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

async Task<IResult> AddReview(int id, CreateReviewDto review, ITourService tourService, HttpContext httpContext)
{
    if (!httpContext.User.Identity?.IsAuthenticated == true)
    {
        return Results.Unauthorized();
    }

    var userNameClaim = httpContext.User.FindFirst(ClaimTypes.Name);
    if (userNameClaim == null)
    {
        return Results.Unauthorized();
    }

    var reviewWithUser = new CreateReviewDto(
        userNameClaim.Value,
        review.Text,
        review.Rating
    );

    try
    {
        var tour = await tourService.AddReviewAsync(id, reviewWithUser);
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

async Task<IResult> GetCurrentUser(HttpContext httpContext, IUserService userService)
{
    if (!httpContext.User.Identity?.IsAuthenticated == true)
    {
        return Results.Unauthorized();
    }

    var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
    {
        return Results.Unauthorized();
    }

    var user = await userService.GetUserByIdAsync(userId);
    return user is null 
        ? Results.NotFound($"User {userId} не найден") 
        : Results.Ok(user);
}

IResult LogoutUser(HttpContext httpContext)
{
    httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
    return Results.Ok();
}

public record AuthRequest(string name, string password);
