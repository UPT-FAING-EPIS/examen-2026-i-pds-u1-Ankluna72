using GymManagement.Application.Interfaces;
using GymManagement.Application.Services;
using GymManagement.Domain.Interfaces;
using GymManagement.Infrastructure.Data;
using GymManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Port config for Render.com (uses PORT env var) ────────────────
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ── Services ─────────────────────────────────────────────────────────────────

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gym Management API",
        Version = "v1",
        Description = "REST API for managing gym clients, memberships, and workout sessions.",
        Contact = new OpenApiContact { Name = "Ankluna72", Email = "contact@gym.com" }
    });
    c.EnableAnnotations();
});

// CORS — allow frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Database — PostgreSQL via EF Core
builder.Services.AddDbContext<GymDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories (DIP — register abstractions with concrete implementations)
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();
builder.Services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();

// Application Services (DIP — services depend on repository abstractions)
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IWorkoutSessionService, WorkoutSessionService>();

// ── App Pipeline ──────────────────────────────────────────────────────────────

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gym Management API v1"));
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();

// Health check endpoint (required by Render.com)
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.MapControllers();

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GymDbContext>();
    db.Database.Migrate();
}

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
