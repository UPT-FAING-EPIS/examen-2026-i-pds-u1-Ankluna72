using GymManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymManagement.IntegrationTests;

/// <summary>
/// Custom WebApplicationFactory that replaces PostgreSQL with SQLite in-memory for integration tests.
/// </summary>
public class GymWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<GymDbContext>));
            if (descriptor is not null) services.Remove(descriptor);

            // Add SQLite in-memory
            services.AddDbContext<GymDbContext>(options =>
                options.UseInMemoryDatabase("GymTestDb"));

            // Ensure DB is created
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
