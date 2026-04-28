namespace GymManagement.Application.DTOs;

/// <summary>
/// DTO for returning a workout session.
/// </summary>
public record WorkoutSessionDto(
    int Id,
    int ClientId,
    string ClientFullName,
    DateTime SessionDate,
    string StartTime,
    string? EndTime,
    string? Notes,
    DateTime CreatedAt,
    IEnumerable<WorkoutExerciseDto> Exercises
);

/// <summary>
/// DTO for creating a new workout session.
/// </summary>
public record CreateWorkoutSessionDto(
    int ClientId,
    DateTime SessionDate,
    string StartTime,
    string? Notes,
    IEnumerable<CreateWorkoutExerciseDto>? Exercises
);

/// <summary>
/// DTO for a single exercise in a session.
/// </summary>
public record WorkoutExerciseDto(
    int Id,
    string ExerciseName,
    int Sets,
    int Reps,
    decimal? WeightKg,
    string? Notes,
    int Order
);

/// <summary>
/// DTO for creating a workout exercise.
/// </summary>
public record CreateWorkoutExerciseDto(
    string ExerciseName,
    int Sets,
    int Reps,
    decimal? WeightKg,
    string? Notes,
    int Order
);
