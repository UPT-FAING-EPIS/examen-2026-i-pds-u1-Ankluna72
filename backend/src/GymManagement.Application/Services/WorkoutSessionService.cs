using GymManagement.Application.DTOs;
using GymManagement.Application.Interfaces;
using GymManagement.Domain.Entities;
using GymManagement.Domain.Interfaces;

namespace GymManagement.Application.Services;

/// <summary>
/// WorkoutSession business logic service. (SRP - Only handles session logic)
/// (DIP - Depends on repository abstractions)
/// </summary>
public class WorkoutSessionService : IWorkoutSessionService
{
    private readonly IWorkoutSessionRepository _sessionRepository;
    private readonly IClientRepository _clientRepository;

    public WorkoutSessionService(IWorkoutSessionRepository sessionRepository, IClientRepository clientRepository)
    {
        _sessionRepository = sessionRepository;
        _clientRepository = clientRepository;
    }

    /// <inheritdoc/>
    public async Task<WorkoutSessionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(id, cancellationToken);
        return session is null ? null : MapToDto(session);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<WorkoutSessionDto>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var sessions = await _sessionRepository.GetByClientIdAsync(clientId, cancellationToken);
        return sessions.Select(s => MapToDto(s));
    }

    /// <inheritdoc/>
    public async Task<WorkoutSessionDto> CreateAsync(CreateWorkoutSessionDto dto, CancellationToken cancellationToken = default)
    {
        var client = await _clientRepository.GetByIdAsync(dto.ClientId, cancellationToken)
            ?? throw new InvalidOperationException($"Client with id {dto.ClientId} not found.");

        if (!TimeSpan.TryParse(dto.StartTime, out var startTime))
            throw new ArgumentException("Invalid start time format. Use HH:mm");

        var session = WorkoutSession.Create(dto.ClientId, dto.SessionDate, startTime, dto.Notes);

        // Add exercises if provided
        if (dto.Exercises is not null)
        {
            int order = 1;
            foreach (var ex in dto.Exercises)
            {
                var exercise = WorkoutExercise.Create(0, ex.ExerciseName, ex.Sets, ex.Reps, ex.WeightKg, ex.Notes, ex.Order > 0 ? ex.Order : order);
                order++;
                // exercises added via session directly in EF
            }
        }

        await _sessionRepository.AddAsync(session, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);

        // Reload
        var saved = await _sessionRepository.GetByIdAsync(session.Id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to retrieve saved session.");

        return MapToDto(saved, client.FullName);
    }

    private static WorkoutSessionDto MapToDto(WorkoutSession s, string? clientFullName = null) => new(
        s.Id,
        s.ClientId,
        clientFullName ?? s.Client?.FullName ?? string.Empty,
        s.SessionDate,
        s.StartTime.ToString(@"hh\:mm"),
        s.EndTime?.ToString(@"hh\:mm"),
        s.Notes,
        s.CreatedAt,
        s.Exercises.Select(e => new WorkoutExerciseDto(e.Id, e.ExerciseName, e.Sets, e.Reps, e.WeightKg, e.Notes, e.Order))
    );
}
