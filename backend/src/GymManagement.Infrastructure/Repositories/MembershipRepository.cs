using GymManagement.Domain.Entities;
using GymManagement.Domain.Interfaces;
using GymManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IMembershipRepository.
/// </summary>
public class MembershipRepository : IMembershipRepository
{
    private readonly GymDbContext _context;

    public MembershipRepository(GymDbContext context)
    {
        _context = context;
    }

    public async Task<Membership?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Memberships
            .Include(m => m.Client)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<IEnumerable<Membership>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default)
        => await _context.Memberships
            .Include(m => m.Client)
            .Where(m => m.ClientId == clientId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Membership membership, CancellationToken cancellationToken = default)
        => await _context.Memberships.AddAsync(membership, cancellationToken);

    public async Task UpdateAsync(Membership membership, CancellationToken cancellationToken = default)
    {
        _context.Memberships.Update(membership);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
