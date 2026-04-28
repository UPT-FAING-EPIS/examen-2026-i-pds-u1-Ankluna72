using GymManagement.Domain.Entities;
using GymManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Data;

/// <summary>
/// Entity Framework Core DbContext for the Gym Management System.
/// </summary>
public class GymDbContext : DbContext
{
    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();
    public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Client configuration
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.LastName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Phone).HasMaxLength(20);
            entity.HasIndex(c => c.Email).IsUnique();
        });

        // Membership configuration
        modelBuilder.Entity<Membership>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.PlanName).IsRequired().HasMaxLength(100);
            entity.Property(m => m.Price).HasColumnType("decimal(10,2)");
            entity.Property(m => m.Status).HasConversion<int>();
            entity.HasOne(m => m.Client)
                  .WithMany(c => c.Memberships)
                  .HasForeignKey(m => m.ClientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // WorkoutSession configuration
        modelBuilder.Entity<WorkoutSession>(entity =>
        {
            entity.HasKey(ws => ws.Id);
            entity.Property(ws => ws.Notes).HasMaxLength(500);
            entity.HasOne(ws => ws.Client)
                  .WithMany(c => c.WorkoutSessions)
                  .HasForeignKey(ws => ws.ClientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // WorkoutExercise configuration
        modelBuilder.Entity<WorkoutExercise>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ExerciseName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.WeightKg).HasColumnType("decimal(8,2)");
            entity.HasOne(e => e.WorkoutSession)
                  .WithMany(ws => ws.Exercises)
                  .HasForeignKey(e => e.WorkoutSessionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
