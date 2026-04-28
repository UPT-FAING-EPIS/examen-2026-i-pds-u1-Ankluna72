using GymManagement.Domain.Entities;

namespace GymManagement.Domain.Interfaces;

/// <summary>
/// Repository contract for WorkoutSession aggregate. (ISP - specific interface)
/// </summary>
public interface IWorkoutSessionRepository
{
    Task<WorkoutSession?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkoutSession>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default);
    Task AddAsync(WorkoutSession session, CancellationToken cancellationToken = default);
    Task UpdateAsync(WorkoutSession session, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
