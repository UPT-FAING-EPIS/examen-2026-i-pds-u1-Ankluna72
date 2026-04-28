namespace GymManagement.Domain.Entities;

/// <summary>
/// Represents a single exercise within a workout session.
/// </summary>
public class WorkoutExercise
{
    public int Id { get; private set; }
    public int WorkoutSessionId { get; private set; }
    public string ExerciseName { get; private set; } = string.Empty;
    public int Sets { get; private set; }
    public int Reps { get; private set; }
    public decimal? WeightKg { get; private set; }
    public string? Notes { get; private set; }
    public int Order { get; private set; }

    // Navigation
    public WorkoutSession? WorkoutSession { get; private set; }

    private WorkoutExercise() { }

    public static WorkoutExercise Create(int workoutSessionId, string exerciseName, int sets, int reps, decimal? weightKg, string? notes, int order)
    {
        if (string.IsNullOrWhiteSpace(exerciseName)) throw new ArgumentException("Exercise name is required.");
        if (sets <= 0) throw new ArgumentException("Sets must be greater than zero.");
        if (reps <= 0) throw new ArgumentException("Reps must be greater than zero.");

        return new WorkoutExercise
        {
            WorkoutSessionId = workoutSessionId,
            ExerciseName = exerciseName,
            Sets = sets,
            Reps = reps,
            WeightKg = weightKg,
            Notes = notes,
            Order = order
        };
    }
}
