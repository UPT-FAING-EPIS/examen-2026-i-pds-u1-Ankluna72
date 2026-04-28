using GymManagement.Application.DTOs;

namespace GymManagement.Application.Interfaces;

/// <summary>
/// Service interface for workout session operations. (DIP)
/// </summary>
public interface IWorkoutSessionService
{
    Task<WorkoutSessionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkoutSessionDto>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default);
    Task<WorkoutSessionDto> CreateAsync(CreateWorkoutSessionDto dto, CancellationToken cancellationToken = default);
}
