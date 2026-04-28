namespace GymManagement.Domain.Entities;

/// <summary>
/// Represents a single workout session for a client.
/// </summary>
public class WorkoutSession
{
    public int Id { get; private set; }
    public int ClientId { get; private set; }
    public DateTime SessionDate { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan? EndTime { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public Client? Client { get; private set; }
    public ICollection<WorkoutExercise> Exercises { get; private set; } = new List<WorkoutExercise>();

    private WorkoutSession() { }

    public static WorkoutSession Create(int clientId, DateTime sessionDate, TimeSpan startTime, string? notes = null)
    {
        return new WorkoutSession
        {
            ClientId = clientId,
            SessionDate = sessionDate.Date,
            StartTime = startTime,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void SetEndTime(TimeSpan endTime) => EndTime = endTime;
    public void UpdateNotes(string notes) => Notes = notes;
}
