using GymManagement.Domain.Entities;
using GymManagement.Domain.Interfaces;
using GymManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IWorkoutSessionRepository.
/// </summary>
public class WorkoutSessionRepository : IWorkoutSessionRepository
{
    private readonly GymDbContext _context;

    public WorkoutSessionRepository(GymDbContext context)
    {
        _context = context;
    }

    public async Task<WorkoutSession?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.WorkoutSessions
            .Include(ws => ws.Client)
            .Include(ws => ws.Exercises)
            .FirstOrDefaultAsync(ws => ws.Id == id, cancellationToken);

    public async Task<IEnumerable<WorkoutSession>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default)
        => await _context.WorkoutSessions
            .Include(ws => ws.Client)
            .Include(ws => ws.Exercises)
            .Where(ws => ws.ClientId == clientId)
            .OrderByDescending(ws => ws.SessionDate)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(WorkoutSession session, CancellationToken cancellationToken = default)
        => await _context.WorkoutSessions.AddAsync(session, cancellationToken);

    public async Task UpdateAsync(WorkoutSession session, CancellationToken cancellationToken = default)
    {
        _context.WorkoutSessions.Update(session);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
